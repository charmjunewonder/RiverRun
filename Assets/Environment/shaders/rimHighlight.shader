Shader "Custom/Rim Highlight" {

	Properties{

		_BumpMap("Normal Map", 2D) = "bump" {}
		_RimColor("Rim Color", Color) = (1.0,0.0,0.0,1.0)
		_RimPower("Rim Range", Range(0.1, 10.0)) =	3.0
		_Hardness("Rim Power", float) = 5.0
	}
	
	SubShader{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" "LightMode" = "ForwardBase" }
		Blend One One 

		Pass{	
		
		CGPROGRAM
	
		#include "UnityCG.cginc"	
		#pragma vertex vert
		#pragma fragment frag	
		
		sampler2D _BumpMap; 
		uniform float4 _RimColor;
		uniform float _RimPower;	
		float _Hardness;
		
		uniform float4 _LightColor0;
		
		struct vertexOutput{
			float4 pos : SV_POSITION;
			float2 uv_BumpMap : TEXCOORD0;
			 fixed3 viewDir : TEXCOORD1;  
			
		};
		
		float4 _BumpMap_ST;
		
		vertexOutput vert(appdata_full v){
			vertexOutput o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv_BumpMap = TRANSFORM_TEX (v.texcoord, _BumpMap);
            TANGENT_SPACE_ROTATION;  
            o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));        
			return o;
		}	
	
		float4 frag(vertexOutput i) : COLOR{
           	float3 norcol = UnpackNormal(tex2D(_BumpMap, i.uv_BumpMap)).rgb; 
			half rim = 1.0 - saturate(dot (normalize(i.viewDir), norcol));
			fixed3 rimColor = saturate((_RimColor.rgb * pow (rim, _RimPower)) * _Hardness);
    		return float4(rimColor, 1.0);  	
		}	
		ENDCG
		}
	}
	Fallback "Diffuse"
}