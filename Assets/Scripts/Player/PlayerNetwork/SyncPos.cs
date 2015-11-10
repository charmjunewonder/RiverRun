using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[NetworkSettings(channel = 1, sendInterval = 0.4f)]
public class SyncPos : NetworkBehaviour {

    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;
    private Vector3 prePos;

	private float lerpRate = 0.1f;
    private float distCovered = 0.0f;

    [SyncVar(hook = "SyncRotationValues")]
    private Quaternion syncRot;
    private Quaternion preRot;
    private float rotCovered = 0.0f;

    void Start()
    {
        lerpRate = 0.02f / 0.5f;
        prePos = transform.position;
        preRot = transform.rotation;
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

        if (rotCovered <= 1)
        {
            rotCovered += lerpRate;
            rotCovered = Mathf.Clamp01(rotCovered);
            transform.rotation = Quaternion.Lerp(preRot, syncRot, rotCovered);
        }
	}

    [Client]
    void SyncPositionValues(Vector3 latestPos)
    {
        distCovered = 0;
        prePos = transform.position;
        syncPos = latestPos;
    }

    [Client]
    void SyncRotationValues(Quaternion latestRot)
    {
        rotCovered = 0;
        preRot = transform.rotation;
        syncRot = latestRot;
    }
	
	void TransmitPosition ()
	{
		if(!isServer) return;
        syncPos = transform.position;
        if (Quaternion.Angle(syncRot, transform.rotation) > 10)
            syncRot = transform.rotation;
	}
}
