using UnityEngine;
using System.Collections;

public class AllianceSpaceshipSpawnController : MonoBehaviour {

    public GameObject alliancePrefab;
    public GameObject enemyPrefab;

    public float radius, range;

    public int maxAllianceNumber;

    private int number;

    

    public void decreaseNumber() { 
        number--;
        Debug.Log("number" + number);
    }

    void Start() {
        for (int i = 0; i < maxAllianceNumber; i++) {
            InstantiateAISpaceship(League.Alliance);
            InstantiateAISpaceship(League.Enemy);
        }
    }

    void Update() {
        if (transform.GetChild(0).childCount < maxAllianceNumber) {
            InstantiateAISpaceship(League.Enemy);
        }
        if (transform.GetChild(1).childCount < maxAllianceNumber) {
            InstantiateAISpaceship(League.Alliance);
        }
    }


    private void InstantiateAISpaceship(League league) {
        int side = Random.Range(0, 3);

       
        Vector3 destination;
        Vector3 startPos;
        Vector3 enter;
        Vector3 exit;

        if (side == 0)
        {
            float angle = Random.Range(-Mathf.PI, Mathf.PI);
            float x = radius * Mathf.Sin(angle);
            float y = radius * Mathf.Cos(angle);

            startPos = new Vector3(x, y, -30);

            angle = Random.Range(-Mathf.PI, Mathf.PI);
            x = radius * Mathf.Sin(angle);
            y = radius * Mathf.Cos(angle);
            
            exit = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 100);
            exit = exit.normalized * 100;

            enter = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 100);
            enter = enter.normalized * 100;

            destination = new Vector3(x, y, 250 + Random.Range(0, range));

        }
        else
        {
            if (side == 1)
            {
                float angle = Random.Range(-Mathf.PI / 2, Mathf.PI / 2);
                float x = radius;
                float y = radius * Mathf.Sin(angle);
                float z = Random.Range(0, range) + radius - radius * Mathf.Cos(angle);

                float aangle = Random.Range(-Mathf.PI, Mathf.PI);
                float xx = -radius;
                float yy = radius * Mathf.Sin(aangle);
                float zz = Random.Range(0, range) + radius + radius * Mathf.Cos(aangle);

                startPos = new Vector3(x, y, z);

                destination = new Vector3(xx, yy, zz);

                exit = new Vector3(-100, Random.Range(-100, 100), Random.Range(-100, 100));
                exit = exit.normalized * 100;

                enter = new Vector3(-100, Random.Range(-100, 100), Random.Range(-100, 100));
                enter = enter.normalized * 100;

            }
            else
            {
                float angle = Random.Range(-Mathf.PI, Mathf.PI);
                float x = radius;
                float y = radius * Mathf.Sin(angle);
                float z = Random.Range(0, range) + radius + radius * Mathf.Cos(angle);

                float aangle = Random.Range(-Mathf.PI / 2, Mathf.PI / 2);
                float xx = -radius;
                float yy = radius * Mathf.Sin(aangle);
                float zz = Random.Range(0, range) + radius - radius * Mathf.Cos(aangle);

                startPos = new Vector3(xx, yy, zz);

                destination = new Vector3(x, y, z);

                exit = new Vector3(100, Random.Range(-100, 100), Random.Range(-100, 100));
                exit = exit.normalized * 100;

                enter = new Vector3(100, Random.Range(-100, 100), Random.Range(-100, 100));
                enter = enter.normalized * 100;
            }
        }

        Bezier myBezier = new Bezier(startPos, exit, enter, destination);
      

        // Chasing mode
        if (Random.Range(0, 2) == 0)
        {
            // Enemy chase Alliance
            if (league == League.Alliance)
            {
                GameObject alliance = Instantiate(alliancePrefab, startPos, Quaternion.identity) as GameObject;
                alliance.GetComponent<AllianceAI>().setBezier(myBezier);
                alliance.GetComponent<AllianceAI>().parentObject = gameObject;
                alliance.GetComponent<AllianceAI>().setLeague(league);
                alliance.transform.parent = transform;

                StartCoroutine(InitializeEnemyChaser(myBezier, startPos, alliance));
            }
            else
            {
                GameObject enemy = Instantiate(enemyPrefab, startPos, Quaternion.identity) as GameObject;
                enemy.GetComponent<AllianceAI>().setBezier(myBezier);
                enemy.GetComponent<AllianceAI>().parentObject = gameObject;
                enemy.GetComponent<AllianceAI>().setLeague(league);
                enemy.transform.parent = transform;

                StartCoroutine(InitializeAllianceChaser(myBezier, startPos, enemy));
            }
        }
        else {
            if (league == League.Alliance)
            {
                GameObject alliance = Instantiate(alliancePrefab, startPos, Quaternion.identity) as GameObject;
                alliance.GetComponent<AllianceAI>().setBezier(myBezier);
                alliance.GetComponent<AllianceAI>().parentObject = gameObject;
                alliance.GetComponent<AllianceAI>().setLeague(league);
                alliance.transform.parent = transform.GetChild(1);
            }
            else {
                GameObject enemy = Instantiate(enemyPrefab, startPos, Quaternion.identity) as GameObject;
                enemy.GetComponent<AllianceAI>().setBezier(myBezier);
                enemy.GetComponent<AllianceAI>().parentObject = gameObject;
                enemy.GetComponent<AllianceAI>().setLeague(league);
                enemy.transform.parent = transform.GetChild(0);
            }
            
        }

        number++;
    }

    IEnumerator InitializeEnemyChaser(Bezier bezier, Vector3 startPos, GameObject chasee) {
        yield return new WaitForSeconds(1.5f);
        
        GameObject enemy = Instantiate(enemyPrefab, startPos, Quaternion.identity) as GameObject;
        enemy.GetComponent<AllianceAI>().setBezier(bezier);
        enemy.GetComponent<AllianceAI>().setLeague(League.Enemy);
        enemy.GetComponent<AllianceAI>().chasee = chasee;
        enemy.transform.parent = transform;
    }

    IEnumerator InitializeAllianceChaser(Bezier bezier, Vector3 startPos, GameObject chasee)
    {
        yield return new WaitForSeconds(1.5f);
        GameObject alliance = Instantiate(alliancePrefab, startPos, Quaternion.identity) as GameObject;
        alliance.GetComponent<AllianceAI>().setBezier(bezier);
        alliance.GetComponent<AllianceAI>().setLeague(League.Alliance);
        alliance.GetComponent<AllianceAI>().chasee = chasee;
        alliance.transform.parent = transform;
    }

}
