using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

[NetworkSettings (channel = 1, sendInterval = 0.2f)]
public class Player_SyncPosition : NetworkBehaviour {

	[SyncVar (hook = "SyncPositionValues")]
	private Vector3 syncPos;

	private float lerpRate;
	private float normalLerpRate = 16;
	private float fasterLerpRate = 27;

	private Vector3 lastPos;
	private float threshold = 0.5f;

	private List<Vector3> syncPosList = new List<Vector3>();
	[SerializeField] private bool useHistoricalLerping = false;
	private float closeEnough = 0.11f;

	void Start ()
	{
		lerpRate = normalLerpRate;
	}

	void Update ()
	{
        //Debug.Log("Count " + syncPosList.Count + " " + lerpRate);
		LerpPosition();
	}

    [Client]
    void SyncPositionValues(Vector3 latestPos)
    {
        syncPos = latestPos;
        syncPosList.Add(syncPos);
    }

	void LerpPosition ()
	{
        if(!isServer){
			if(useHistoricalLerping)
			{
				HistoricalLerping();
			}
			else
			{
				OrdinaryLerping();
			}
        }
        else
        {
            syncPos = transform.position;
        }
	}

	void OrdinaryLerping ()
	{
        transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime * lerpRate);
	}

	void HistoricalLerping ()
	{
		if(syncPosList.Count > 0)
		{
            transform.position = Vector3.Lerp(transform.position, syncPosList[0], Time.deltaTime * lerpRate);

            if (Vector3.Distance(transform.position, syncPosList[0]) < closeEnough)
			{
				syncPosList.RemoveAt(0);
			}

			if(syncPosList.Count > 10)
			{
				lerpRate = fasterLerpRate;
			}
			else
			{
				lerpRate = normalLerpRate;
			}

			//Debug.Log(syncPosList.Count.ToString());
		}
	}
}
