using UnityEngine;
using System.Collections;

public class ServerCameraMovement : MonoBehaviour {

    private float angle;
    private int flag;
    private float radius;

	void Start () {
        flag = 1;
	    angle = -90;
        radius = Vector3.Distance(transform.position, new Vector3(0, transform.position.y, 0));
	}
	
	void Update () {

        angle += flag * Time.deltaTime;

        Vector3 pos = transform.position;
        transform.position = new Vector3(radius * Mathf.Cos(angle * Mathf.PI / 180), pos.y, radius * Mathf.Sin(angle * Mathf.PI / 180));
        transform.LookAt(new Vector3(0, 0, 0));

        if (angle >= -80 || angle < -100) {
            flag *= -1;
        } 
	}
}
