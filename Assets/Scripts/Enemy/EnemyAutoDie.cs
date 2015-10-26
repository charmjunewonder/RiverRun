using UnityEngine;
using System.Collections;

public class EnemyAutoDie : MonoBehaviour {

    public float dieTime = 3.0f;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        dieTime -= Time.deltaTime;

        if (dieTime <= 0) {
            Destroy(gameObject);
        }
	}
}
