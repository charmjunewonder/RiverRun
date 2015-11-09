using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShieldCollisionBehaviour : NetworkBehaviour
{
    public PlayerController playerController;
    public GameObject effect, explosion;
    public Vector3 FixInctancePosition, FixInctanceAngle;
    public float FixInctanceScalePercent = 100;
    public bool IsDefaultCollisionPoint;

    private int count;
    public float countDown;
    public bool countDownFlag;
   

    private Vector3 pos;
    // Use this for initialization
    void Start () {
        countDownFlag |= false;
    }
	
    // Update is called once per frame
    void Update () {
        if(!isServer) return;
	    if(!countDownFlag) return;

        countDown -= Time.deltaTime;
        if (countDown <= 0) {
            NetworkServer.Destroy(gameObject);
            playerController.CloseShield();
        }

    }
    void OnTriggerEnter(Collider collider)
    {
        if (isServer) {
            count--;
            if(count <= 0)
            {
                NetworkServer.Destroy(gameObject);
                playerController.CloseShield();
            }
                
        }
        pos = transform.position;
        Vector3 hitPoint = Vector3.zero;
        if (!IsDefaultCollisionPoint)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, (collider.transform.position - pos).normalized, out hit);
            hitPoint = hit.point;
        }
        
        if (effect!=null) {
           
            var part = effect.GetComponent<ParticleSystem>();
            if (part!=null) {
                part.startSize = transform.lossyScale.x;
            }
            else {
                effect.transform.localScale = transform.lossyScale;
            }
            var inst1 = Instantiate(effect) as GameObject;
            Debug.Log("inst1 " + inst1);
            inst1.transform.parent = gameObject.transform;
            inst1.transform.localPosition = new Vector3(0, 0, 0) + FixInctancePosition;
            if (IsDefaultCollisionPoint) inst1.transform.localRotation = Quaternion.identity;
            else
                inst1.transform.LookAt(hitPoint);
            inst1.transform.Rotate(FixInctanceAngle);
            //inst1.transform.localScale = transform.localScale * FixInctanceScalePercent / 100f;
        }
        if (explosion != null)
        {
            var inst2 = Instantiate(explosion, hitPoint, new Quaternion()) as GameObject;
            inst2.transform.parent = transform;
        }
    }

    public void setMaximumDefendNumber(int num) {
        count = num;
    }

    public void setCountDown(float cd) {
        countDown = cd;
        countDownFlag = true;
    }
}
