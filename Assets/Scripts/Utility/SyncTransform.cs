using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncTransform : NetworkBehaviour {

    [SyncVar (hook = "SyncPosition")]
    private Vector3 pos;

    [SyncVar]
    private Vector3 scale;

    [SyncVar]
    private Quaternion q;


	void Start () {

        if(!isServer){
            transform.rotation = q;

            transform.localScale = scale;
        }
	}

    private void SyncPosition(Vector3 pos) {
        if(!isServer)
            transform.position = pos;
    }


    public void setTransform(Transform t) {
        pos = t.position;

        q = t.rotation;

        scale = t.localScale;
        
    
    }

}
