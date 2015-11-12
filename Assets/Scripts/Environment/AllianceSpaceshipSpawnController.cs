using UnityEngine;
using System.Collections;

public class AllianceSpaceshipSpawnController : MonoBehaviour {

    public GameObject alliancePrefab;

    public float radius, range;

    public int maxAllianceNumber;

    private int number;

    

    public void decreaseNumber() { 
        number--;
    }

    void Start() {
        for (int i = 0; i < maxAllianceNumber; i++) {
            number++;
            GenerateEnemy();
        }
    }

    void Update() {
        if (number < maxAllianceNumber) {
            number++;
            GenerateEnemy();
        }
    }

    private void GenerateEnemy()
    {
        int side = Random.Range(0, 3);

        Vector3 pos;
        Vector3 toLookAt;
        float angle = Random.Range(0, 180);

        if (side == 0)
        {
            pos = new Vector3(Random.Range(-50, 50), Random.Range(-30, 30), -30);

            toLookAt = new Vector3(pos.x, pos.y, 1000);

        }
        else {
            
            float z = Random.Range(30, 600);

            side = Random.Range(0, 2);

            if (side == 0) {

                float x = -z * 0.6f;

                pos = new Vector3(x, Random.Range(-30, 30), z);

                toLookAt = new Vector3(-x + Random.Range(-50, 50), Random.Range(-10, 10), z + Random.Range(-50, 50));
            }
            else {
                float x = z * 0.6f;

                pos = new Vector3(x, Random.Range(-30, 30), z);

                toLookAt = new Vector3(-x + Random.Range(-50, 50), Random.Range(-10, 10), z + Random.Range(-50, 50));
            }
        }

        GameObject alliance = Instantiate(alliancePrefab, pos, Quaternion.identity) as GameObject;

        alliance.transform.LookAt(toLookAt);

        AllianceAI ai = alliance.GetComponent<AllianceAI>();

        ai.speed = Random.Range(30f, 70f);
        ai.turnAngle = angle;
        ai.parentObj = gameObject;
    }
}
