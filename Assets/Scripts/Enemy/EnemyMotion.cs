using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyMotion : NetworkBehaviour {

    public GameObject bullet;

    private Vector3 velocity;
    private GameObject spaceship;
    private GameObject enemySkills;
    private float skillTimer;



    public void setSpaceship(GameObject ss) { spaceship = ss; }
    public void setEnemySkills(GameObject es) { enemySkills = es; }


	void Start () {
        if (isServer) {
            spaceship = transform.parent.GetComponent<EnemySpawnManager>().GetSpaceShip();

            velocity = Vector3.Normalize(spaceship.transform.position - transform.position) * Random.Range(0.1f, 0.15f);

            skillTimer = Random.Range(3.0f, 8.0f);
        }

	}
	
	void Update () {
        if (isServer) {
            if (Vector3.Distance(spaceship.transform.position, transform.position) > 100){
                transform.position += velocity;
            }
        }

	}
}
