using UnityEngine;
using System.Collections;

public enum League { Alliance, Enemy };

public class AllianceAI : MonoBehaviour {

    public GameObject parentObj;
    public float speed;
    public float turnAngle;
    public int turnDirection;
    public float turnTime;

    private bool beingDestroyed;

    private float freezeTimer;
    private Vector3 prevVelocity;

    void Start() {
        turnTime = Random.Range(1.0f, 5.0f);
        int t = Random.Range(0, 2);
        turnDirection = t == 0 ? -1 : 1;
        beingDestroyed = false;
        freezeTimer = -1;
    }

    void Update() {

        if (freezeTimer >= 0) {
            freezeTimer -= Time.deltaTime;
            return;
        }
        if (prevVelocity != Vector3.zero)
        {
            GetComponent<Rigidbody>().velocity = prevVelocity;
            prevVelocity = Vector3.zero;
        }

        if (beingDestroyed) {
            return;
        }
        if (turnAngle < 0 && !beingDestroyed)
        {
            if (transform.position.z > 600 || Mathf.Abs(transform.position.x) > Mathf.Abs(transform.position.z)
            || transform.position.z < -50)
            {
                beingDestroyed = true;
                DestroySelf();
            }
        }


        Vector3 direction = transform.TransformDirection(Vector3.forward);
        GetComponent<Rigidbody>().velocity = direction * speed;

        if (turnTime > 0)
        {
            turnTime -= Time.deltaTime;
            return;
        }

        if (turnAngle >= 0) {
            turnAngle -= Time.deltaTime * 20;
            transform.Rotate(0, -turnDirection * Time.deltaTime * 20, 0, Space.World);
        }
    }

    public void Freeze(float t) { 
        freezeTimer = t; 
        prevVelocity = GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void DestroySelf() {
        parentObj.GetComponent<AllianceSpaceshipSpawnController>().decreaseNumber();
        transform.GetChild(0).gameObject.SetActive(false);
        Invoke("goDestroy", 3.0f);
    }

    private void goDestroy() {
        Destroy(gameObject);
    }

}
