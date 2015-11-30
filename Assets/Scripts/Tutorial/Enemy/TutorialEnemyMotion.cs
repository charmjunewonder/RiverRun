using UnityEngine;
using System.Collections;

public class TutorialEnemyMotion : MonoBehaviour {

	void Start () {
	    transform.LookAt(new Vector3(0, 3, -5));
	}
	
	void Update () {
        Vector3 direction = transform.TransformDirection(Vector3.forward);

        GetComponent<Rigidbody>().velocity = direction;
	}
}
