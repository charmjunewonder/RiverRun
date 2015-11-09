using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncTransform : NetworkBehaviour {

    [SyncVar]
    private Vector3 pos;

    [SyncVar]
    private Vector3 scale;

    [SyncVar]
    private Quaternion q;


	void Start () {

        if(!isServer){
            transform.position = pos;

            transform.rotation = q;

            transform.localScale = scale;
        }
	}
	

    public void setTransform(Transform t) {
        pos = t.position;

        q = t.rotation;

        scale = t.localScale;
        
    
    }

}
