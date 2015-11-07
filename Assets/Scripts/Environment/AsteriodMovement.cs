using UnityEngine;
using System.Collections;

public class AsteriodMovement : MonoBehaviour {

    float x, y, z;

    Vector3 velocity;

	void Start () {
        x = Random.Range(-1f, 1f);
        y = Random.Range(-1f, 1f);
        z = Random.Range(-1f, 1f);

        velocity = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        velocity = velocity.normalized;
	}
	
	// Update is called once per frame
	void Update () {
	    transform.Rotate(Time.deltaTime * x, Time.deltaTime * y, Time.deltaTime * z);

        transform.position += velocity * Time.deltaTime;

	}
}
