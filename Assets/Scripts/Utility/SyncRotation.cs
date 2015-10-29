using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncRotation : NetworkBehaviour
{
    //[SyncVar]
    //public Quaternion startRot;

    // Use this for initialization
    void Start()
    {
        //transform.rotation = startRot;
        transform.LookAt(new Vector3(0, 0, 1));

    }
}
