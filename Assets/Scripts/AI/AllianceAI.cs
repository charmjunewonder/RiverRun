using UnityEngine;
using System.Collections;

public enum League { Alliance, Enemy };

public class AllianceAI : MonoBehaviour {

    public GameObject spaceship;
    public GameObject parentObject;
    public GameObject disintegrationPrefab;
    public GameObject attackPrefab;
    public GameObject chasee;

    
    Bezier myBezier;

    enum FlyMode { stright, bezier };

    private float t;
    private League league;
    private bool go;
    private FlyMode mode;

    private bool missileLaunched;
    private Vector3 destination;

	void Start () {
        t = 0.0f;
        go = go | false;
        missileLaunched = false;
        mode = FlyMode.bezier;
	}
	
	void Update () {

        if (!go) return;

        t += 0.001f;

        if (chasee != null){
            if (!missileLaunched){
                if (t >= 0.35f){
                    missileLaunched = true;
                    GameObject attack = Instantiate(attackPrefab, transform.position, Quaternion.identity) as GameObject;
                    attack.GetComponent<AISkillTracingController>().speed = 3f;
                    attack.GetComponent<AISkillTracingController>().chasee = chasee;
                   
                }
            }
            else {
                float angle = Vector3.Angle(transform.TransformDirection(Vector3.forward), chasee.transform.position - transform.position);
                if (angle > -30 && angle < 30) {
                    Vector3 velocity = (chasee.transform.position - transform.position).normalized * 1.2f;
                    transform.position += velocity;
                    transform.LookAt(chasee.transform.position);

                    if (Vector3.Distance(transform.position, chasee.transform.position) < 100) {
                        GameObject attack = Instantiate(attackPrefab, transform.position, Quaternion.identity) as GameObject;
                        attack.GetComponent<AISkillTracingController>().speed = 3f;
                        attack.GetComponent<AISkillTracingController>().chasee = chasee;
                        FindNextTarget();
                    }
                    return;
                }
            }
        }
        else{
            if(t >0.3f)
                FindNextTarget();

        }

        transform.position = myBezier.getPosition(t);

        Vector3 nextPos = myBezier.getPosition(t + 0.001f);

        transform.LookAt(nextPos);

        if (t >= 0.96f) {
            FindNextTarget();
        }
	}

    private void FindNextTarget() {

        if (transform.parent.gameObject.name.Equals("AllianceSpaceshipObject"))
        {
            if (league == League.Alliance)
                transform.parent = transform.parent.GetChild(1);
            else
                transform.parent = transform.parent.GetChild(0);
        }

        if (league == League.Alliance){
            Transform opponentParent = transform.parent.parent.GetChild(0);
            chasee = opponentParent.GetChild(Random.Range(0, opponentParent.childCount)).gameObject;
        }
        else {
            Transform opponentParent = transform.parent.parent.GetChild(1);
            chasee = opponentParent.GetChild(Random.Range(0, opponentParent.childCount)).gameObject;
        }

        destination = chasee.transform.position;

        myBezier = new Bezier(transform.position, transform.TransformDirection(transform.forward).normalized * 300, 
                                (destination - transform.position).normalized * 10, destination);
        t = 0.0f;
    }

    public void setBezier(Bezier mb) {
        myBezier = mb;
        go = true;

        destination = mb.getDestination();

        Vector3 nextPos = myBezier.getPosition(t + 0.001f);

        transform.LookAt(nextPos);
    }

    public void setLeague(League l) {
        league = l;
    }

    public void Explode() {
        
        if(!go) return;
        
        go = false;

        transform.GetChild(0).gameObject.SetActive(false);
        Instantiate(disintegrationPrefab, transform.position, Quaternion.identity);
        Invoke("InvokeDestroy", 3.0f);
    }

    public void Disappear() {
        go = false;

        transform.GetChild(0).gameObject.SetActive(false);
        Invoke("InvokeDestroy", 3.0f);
    }

    public Vector3 getDestination() {
        if (mode == FlyMode.bezier)
            return destination;
        else
            return transform.position + transform.TransformDirection(transform.forward).normalized * 150;
    }

    private void InvokeDestroy(){
        Destroy(gameObject);
    }

    private Vector3 RandomizeFactorVector3(float low, float high)
    {
        float ax = Random.Range(low, high), ay = Random.Range(low, high), az = Random.Range(low, high);
        return new Vector3(ax, ay, az);
    }

}
