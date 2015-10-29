using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[NetworkSettings(channel = 1, sendInterval = 0.5f)]
public class SyncPos : NetworkBehaviour {

    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;
    private Vector3 prePos;

	private float lerpRate = 0.1f;
    private float distCovered = 0.0f;
    private int count = 1;
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
	}

    [Client]
    void SyncPositionValues(Vector3 latestPos)
    {
        //Debug.Log("fsdfk "+distCovered);
        distCovered = 0;
        //transform.position = prePos;
        prePos = syncPos;
        syncPos = latestPos;
    }
	
	void TransmitPosition ()
	{
		if(!isServer) return;
        syncPos = transform.position;
	}
}
