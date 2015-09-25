using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncShipPos : NetworkBehaviour {
	[SyncVar]
	private Vector3 syncPos;
	
	private Transform myTransform;

	private float lerpRate = 15;
	
	// Use this for initialization
	void Start () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void FixedUpdate () 
	{
		TransmitPosition();
		LerpPosition();
	}
	
	void LerpPosition ()
	{
		if(isServer) return;
		myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
	}
	
	
	void TransmitPosition ()
	{
		if(!isServer) return;
		syncPos = myTransform.position;
	}
}