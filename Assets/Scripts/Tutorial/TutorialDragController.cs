using UnityEngine;
using System.Collections;

public class TutorialDragController : MonoBehaviour {

    public Vector3 start, end;

    protected Vector3 restart;
    protected Vector3 speed;

    float timer;

	void Start () {
        timer = 1;
	    speed = (end - start).normalized;
        restart = transform.position;
	}
	
	void Update () {

        timer -= Time.deltaTime;

        transform.position += speed * 0.5f;

        if (timer <= 0) {
            transform.position = restart;
            timer = 1f;
        }
	}
}
