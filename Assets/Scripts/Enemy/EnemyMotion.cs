using UnityEngine;
using System.Collections;

public class EnemyMotion : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
	}
}
