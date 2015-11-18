Shader "UI/UI_Flash"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		//_AnimatedTex ("Animated Texture",2D) = "white"{}
		[MaterialToggle] _Trigger("_Trigger(Int)", Int) = 1
		_Speed("HighLight Speed",Range(0,1)) = 0.3
		_Power("HighLight Power",Range(0,1)) = 0.5
		_Color ("Tint", Color) = (1,1,1,1)
		_HighColor("HightLight Color",Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]
		
		Pass
		{

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
	
			bool _UseClipRect;
			float4 _ClipRect;

			bool _UseAlphaClip;
		

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = mul(UNITY_MATRIX_MVP, OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
		
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
				#endif

				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			//sampler2D _AnimatedTex;
			half _Speed,_Power;
			fixed4 _HighColor;
			bool _Trigger;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 baseColor = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				if (_UseClipRect)
					baseColor *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				
				if (_UseAlphaClip)
					clip (baseColor.a - 0.001);

				half u_loc = clamp (abs(frac(_Time.y*_Speed)-0.5)*2.0-0.25,0.0,0.5) *2.0;
				half v_loc = clamp (abs(frac(_Time.y*_Speed+0.25)-0.5)*2.0-0.25,0.0,0.5) *2.0;
				half u_loc_2 = clamp (abs(frac(_Time.y*_Speed+0.5)-0.5)*2.0-0.25,0.0,0.5) *2.0;
				half v_loc_2 = clamp (abs(frac(_Time.y*_Speed+0.75)-0.5)*2.0-0.25,0.0,0.5) *2.0;
				//bool isIn = (abs(u_loc-IN.texcoord.x)<0.04)*(abs(v_loc-IN.texcoord.y)<0.04)+
				//              (abs(u_loc_2-IN.texcoord.x)<0.04)*(abs(v_loc_2-IN.texcoord.y)<0.04);
			    half weight = clamp(0.07-abs(u_loc-IN.texcoord.x),0,0.07)*clamp(0.07-abs(v_loc-IN.texcoord.y),0,0.07)
				             + clamp(0.07-abs(u_loc_2-IN.texcoord.x),0,0.07)*clamp(0.07-abs(v_loc_2-IN.texcoord.y),0,0.07);
				
				
				//half u_high = clamp (abs(frac(_Time.y*_Speed)-0.5)*2.0-0.25,0.01,0.49) *2.0;
				//half v_high = clamp (abs(frac(_Time.y*_Speed+0.25)-0.5)*2.0-0.25,0.01,0.49) *2.0;
				//half u_high_2 = clamp (abs(frac(_Time.y*_Speed+0.5)-0.5)*2.0-0.25,0.01,0.49) *2.0;
				//half v_high_2 = clamp (abs(frac(_Time.y*_Speed+0.75)-0.5)*2.0-0.25,0.01,0.49) *2.0;
				//bool isIn = (abs(u_high-IN.texcoord.x)<0.04)*(abs(v_high-IN.texcoord.y)<0.04)+
				//              (abs(u_high_2-IN.texcoord.x)<0.04)*(abs(v_high_2-IN.texcoord.y)<0.04);
			   
				//isIn = isIn*_Trigger;
				//half4 color = baseColor*(!isIn)+isIn*_HighColor*_Power;
				//weight = weight+isIn*0.5;
				half4 color = baseColor+_Trigger*weight*_HighColor*_Power*800;
				color = half4 (color.r,color.g,color.b,baseColor.a);
				return color;
			}
		ENDCG
		}
	}
}
