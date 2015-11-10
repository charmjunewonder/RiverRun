﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[NetworkSettings(channel = 1, sendInterval = 0.4f)]
public class SyncPos : NetworkBehaviour {

    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;
    private Vector3 prePos;

	private float lerpRate = 0.1f;
    private int count = 1;
    private float distCovered = 0.0f;
    private float currentTime = 0f;
    private float timeToMove = 2f;
    void Start()
    {
        lerpRate = 0.02f / 0.5f;
        prePos = transform.position;
    }

	void FixedUpdate () 
	{
		TransmitPosition();
		LerpPosition();
	}
	
	void LerpPosition ()
	{
		if(isServer) return;
        distCovered += lerpRate;
        distCovered = Mathf.Clamp01(distCovered);
        transform.position = Vector3.Lerp(prePos, syncPos, distCovered);
        //currentTime += 0.02f;
        //if (currentTime <= timeToMove)
        //{
        //    transform.position = Vector3.Lerp(prePos, syncPos, currentTime / timeToMove);
        //}
        //else
        //{
        //    //timeToMove = currentTime;
        //    //currentTime = 0;
        //    //Vector3 dist = syncPos - prePos;
        //    //prePos = transform.position;
        //    //syncPos = syncPos + dist.normalized * 10;
        //    //Debug.Log(prePos + " " + syncPos);
        //}
        //Debug.Log(currentTime + " " + timeToMove + " " + currentTime / timeToMove);
        
	}

    [Client]
    void SyncPositionValues(Vector3 latestPos)
    {
        distCovered = 0;
        //Debug.Log("fsdfk "+distCovered);
        //timeToMove = currentTime;
        //currentTime = 0;
        //transform.position = prePos;
        prePos = transform.position;
        syncPos = latestPos;
    }
	
	void TransmitPosition ()
	{
		if(!isServer) return;
        syncPos = transform.position;
	}
}
