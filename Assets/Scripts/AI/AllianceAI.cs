using UnityEngine;
using System.Collections;

public class AllianceAI : MonoBehaviour {

    public GameObject spaceship;
    public GameObject parentObject;
    public GameObject disintegrationPrefab;

    Bezier myBezier;

    public Vector3 destination;

    private float t;
    public bool go;


	void Start () {
        t = 0.0f;
        go = go | false;

        myBezier = new Bezier(transform.position, RandomizeFactorVector3(-200.0f, 200.0f), RandomizeFactorVector3(-200.0f, 200.0f), destination);

        Vector3 nextPos = myBezier.getPosition(t + 0.002f);

        transform.LookAt(nextPos);

	}
	
	void Update () {

        if (!go) return;

        t += 0.002f;

        transform.position = myBezier.getPosition(t);

        Vector3 nextPos = myBezier.getPosition(t + 0.002f);

        transform.LookAt(nextPos);

        if (t >= 0.96f) {
            parentObject.GetComponent<AllianceSpaceshipSpawnController>().decreaseNumber();
            transform.GetChild(0).gameObject.SetActive(false);
            Instantiate(disintegrationPrefab, transform.position, Quaternion.identity);
            go = false;
            Invoke("InvokeDestroy", 3.0f);
            
        }
	}

    public void setDestination(Vector3 des) {
        destination = des;
        go = true;
        
    }

    private Vector3 RandomizeFactorVector3(float low, float high) {
        float ax = Random.Range(low, high), ay = Random.Range(low, high), az = Random.Range(low, high);
        return new Vector3(ax, ay, az);
    }


    private void InvokeDestroy(){
        Destroy(gameObject);
    }
}
