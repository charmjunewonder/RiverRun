using UnityEngine;
using System.Collections;

public class EnemyObjectController : MonoBehaviour {

    public GameObject spaceship;

    public GameObject[] enemies;

    void Start() {
        Vector3 centerPos = spaceship.transform.position;

        foreach (GameObject e in enemies) {
            e.transform.position = new Vector3(Random.Range(-50, 50), Random.Range(-25, 25), Random.Range(120, 180)) + centerPos;
        }
    
    }


}
