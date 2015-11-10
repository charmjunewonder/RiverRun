using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncTransformLightening : NetworkBehaviour
{

    [SyncVar]
    private Vector3 pos1;

    [SyncVar]
    private Vector3 scale1;

    [SyncVar]
    private Quaternion q1;

    [SyncVar]
    private Vector3 pos2;

    [SyncVar]
    private Vector3 scale2;

    [SyncVar]
    private Quaternion q2;


    void Start()
    {
        if (!isServer)
        {
            transform.GetChild(0).position = pos1;

            transform.GetChild(0).rotation = q1;

            transform.GetChild(0).localScale = scale1;

            transform.GetChild(1).position = pos2;

            transform.GetChild(1).rotation = q2;

            transform.GetChild(1).localScale = scale2;
        }
    }


    public void setTransform(Transform t1, Transform t2)
    {
        pos1 = t1.position;

        q1 = t1.rotation;

        scale1 = t1.localScale;

        pos2 = t2.position;

        q2 = t2.rotation;

        scale2 = t2.localScale;

    }
	
}
