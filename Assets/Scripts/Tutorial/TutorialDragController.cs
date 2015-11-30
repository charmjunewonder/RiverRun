using UnityEngine;
using System.Collections;

public class TutorialDragController : MonoBehaviour {

    public Vector3 start, end;

    public Vector3 restart;
    public Vector3 speed;

    public float timer;
    private float t;

	void Start () {
        t = timer;
	    speed = (end - start).normalized;
        restart = transform.position;
	}
	
	void Update () {

        t -= Time.deltaTime;
#if UNITY_STANDALONE_WIN
        transform.position += speed;
#endif

#if UNITY_IOS
        transform.position += speed * 3;
#endif

        if (t <= 0) {
            transform.position = restart;
            t = timer;
        }
	}
}
