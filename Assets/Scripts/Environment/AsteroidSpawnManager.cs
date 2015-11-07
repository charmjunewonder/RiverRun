using UnityEngine;
using System.Collections;

public class AsteroidSpawnManager : MonoBehaviour {

    public GameObject[] asteroids;
	
	void Start () {
        int num = Random.Range(30, 40);

        for (int i = 0; i < num; i++) {

            Vector3 pos = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(0, 200));

            GameObject ast = GameObject.Instantiate(asteroids[Random.Range(0,15)], pos, Quaternion.identity) as GameObject;

            ast.transform.parent = transform;
        }
	}
	
	void Update () {
	
	}
}
