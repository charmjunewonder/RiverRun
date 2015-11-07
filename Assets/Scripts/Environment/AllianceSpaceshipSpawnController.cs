using UnityEngine;
using System.Collections;

public class AllianceSpaceshipSpawnController : MonoBehaviour {

    public GameObject alliancePrefab;

    public float radius, range;

    public int maxAllianceNumber;

    private int number;

    

    public void decreaseNumber() { number--; }

    void Start() {
        for (int i = 0; i < maxAllianceNumber; i++) {
            InstantiateAlliance();
        }
    }

    void Update() {
        if (number < maxAllianceNumber) {
            InstantiateAlliance();
        }
    }


    private void InstantiateAlliance() {
        int side = Random.Range(0, 3);
        if (side == 0)
        {
            float angle = Random.Range(-Mathf.PI, Mathf.PI);
            float x = radius * Mathf.Sin(angle);
            float y = radius * Mathf.Cos(angle);
            GameObject alliance = Instantiate(alliancePrefab, new Vector3(x, y, -30), Quaternion.identity) as GameObject;

            angle = Random.Range(-Mathf.PI, Mathf.PI);
            x = radius * Mathf.Sin(angle);
            y = radius * Mathf.Cos(angle);

            Vector3 destination = new Vector3(x, y, 250 + Random.Range(0, range));

            alliance.GetComponent<AllianceAI>().setDestination(destination);

            alliance.GetComponent<AllianceAI>().parentObject = gameObject;

            alliance.transform.parent = transform;
        }
        else
        {
            float angle = Random.Range(-Mathf.PI, Mathf.PI);
            float x = radius;
            float y = radius * Mathf.Cos(angle);
            float z = Random.Range(0, range) + radius - radius * Mathf.Sin(angle);

            float aangle = Random.Range(-Mathf.PI, Mathf.PI);
            float xx = -radius;
            float yy = radius * Mathf.Cos(aangle);
            float zz = Random.Range(0, range) + radius - radius * Mathf.Sin(aangle);

            if (side == 1)
            {
                GameObject alliance = Instantiate(alliancePrefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;

                Vector3 destination = new Vector3(xx, yy, zz);

                alliance.GetComponent<AllianceAI>().setDestination(destination);

                alliance.GetComponent<AllianceAI>().parentObject = gameObject;

                alliance.transform.parent = transform;
            }
            else
            {
                GameObject alliance = Instantiate(alliancePrefab, new Vector3(xx, yy, zz), Quaternion.identity) as GameObject;

                Vector3 destination = new Vector3(x, y, z);

                alliance.GetComponent<AllianceAI>().setDestination(destination);

                alliance.GetComponent<AllianceAI>().parentObject = gameObject;

                alliance.transform.parent = transform;

            }
        }
        number++;
    }

}
