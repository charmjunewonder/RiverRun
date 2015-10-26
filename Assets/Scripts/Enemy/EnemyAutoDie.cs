using UnityEngine;
using System.Collections;

public class EnemyAutoDie : MonoBehaviour {

    public float dieTime = 3.0f;

	void Start () {
	
	}
	
	void Update () {
        dieTime -= Time.deltaTime;

        if (dieTime <= 0) {
            Destroy(gameObject);
        }
	}
}
