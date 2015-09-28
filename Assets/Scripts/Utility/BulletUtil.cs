using UnityEngine;
using System.Collections;

public class BulletUtil : MonoBehaviour {

    private float timer;

	void Start () {
        timer = 4.0f;
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            Destroy(gameObject);
        }
	}
}
