using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AutoMove : NetworkBehaviour {
    [SyncVar]
    public Vector3 startPos;
    [SyncVar]
    public Vector3 velocity;

	// Use this for initialization
	void Start () {
        transform.position = startPos;
	}
	
	// Update is called once per frame
	void Update () {
        if(!isServer)
            transform.position = transform.position + velocity * Time.deltaTime;
	}
}
