using UnityEngine;
using System.Collections;

public class ShieldCollisionBehaviour : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject effect, explosion;
    public Vector3 FixInctancePosition, FixInctanceAngle;
    public float FixInctanceScalePercent = 100;
    public bool IsDefaultCollisionPoint;
    public int maxCount;
    public int count;
    public float countDown;
    public bool countDownFlag;
   

    private Vector3 pos;
    // Use this for initialization
    void Start () {
        countDownFlag |= false;
        FixInctancePosition = new Vector3(0, 0, 0.1f);
    }
	
    // Update is called once per frame
    void Update () {
	    if(!countDownFlag) return;
        if(!playerController.CheckServer()) return;

        countDown -= Time.deltaTime;
        if (countDown <= 0) {
            
            playerController.skill1Counter += (maxCount-count);
            playerController.score += ScoreParameter.Defender_Skill1_Score * (maxCount - count);
            playerController.HideShield();
            countDownFlag = false;
        }

    }
    void OnTriggerEnter(Collider collider)
    {
        if (playerController.CheckServer())
        {
            count--;
            if(count <= 0)
            {
                Debug.Log("What's going on");
                playerController.skill1Counter += maxCount;
                playerController.score += ScoreParameter.Defender_Skill1_Score * maxCount;
                playerController.HideShield();
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
            
            var inst1 = Instantiate(effect, transform.position, transform.rotation) as GameObject;
            
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
        maxCount = num;
    }

    public void setCountDown(float cd) {
        countDown = cd;
        countDownFlag = true;
    }
}
