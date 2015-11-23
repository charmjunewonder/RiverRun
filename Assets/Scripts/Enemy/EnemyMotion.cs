using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyMotion : NetworkBehaviour {

    public GameObject bulletPref;
    public GameObject dieParticlePref;

    [SyncVar]
    public int index;

    protected Vector3 velocity;
    public Vector3 destination;

    private GameObject spaceship;
    protected GameObject enemySkillManager;

    protected float skillTimer;

    [SyncVar]
    protected float blood;
    [SyncVar]
    protected float max_blood;

    protected int damage;
    protected float attackTime;

    [SyncVar]
    private int flyStatus;


    private int turnDirection;
    private int turnTimer;
    private float turnAngle;
    private float finishTurnAngle;
    private float turnningSpeed;
    private float turnAngleZ;
    
    
    protected float freezeTimer;
    public Vector3 prevVelocity;

    public float recomeAngle;

    private Vector3 pz = new Vector3(0, 0, 1000);
    private Vector3 nz = new Vector3(0, 0, -1000);

    public void setEnemySkills(GameObject es) { enemySkillManager = es; }
    public void setBlood(float b) { blood = b; }
    public float getBlood() { return blood; }
    public void setMaxBlood(float b) { max_blood = b; }
    public float getMaxBlood() { return max_blood; }
    public void setIndex(int ind) { index = ind; }
    public void setDamage(int d) { damage = d; }
    public void setAttackTime(float t) { attackTime = t; skillTimer = attackTime; }

    public void DecreaseBlood(float damage) {
        blood -= damage;
        Debug.Log("CmdDecreaseBlood " + blood);
        if (blood <= 0)
        {
            transform.parent.GetComponent<EnemySpawnManager>().AddProgress(1);

            GameObject particle = Instantiate(dieParticlePref, gameObject.transform.position, Quaternion.identity) as GameObject;

            RpcCreateDieParticle();

            NetworkServer.Destroy(gameObject);
        }
    }

    [ClientRpc]
    void RpcCreateDieParticle() {
        GameObject particle = Instantiate(dieParticlePref, gameObject.transform.position, Quaternion.identity) as GameObject;
    }

	void Start () {
        if (isServer) {
            spaceship = transform.parent.GetComponent<EnemySpawnManager>().GetSpaceShip();

            float radian = Random.Range(0, 2 * Mathf.PI);

            destination = spaceship.transform.position + new Vector3(40 * Mathf.Cos(radian), 40 * Mathf.Sin(radian), 80);

            transform.LookAt(destination);

            skillTimer = 2;

            turnDirection = Random.Range(0, 2);
            turnDirection = turnDirection == 0 ? -1 : 1;
            flyStatus = -1;
            turnAngle = 0;
            turnTimer = 0;
            turnAngleZ = 0.0f;

            freezeTimer = -1;
        }

	}
	
	void Update () {
        if (isServer)
        {
            if (freezeTimer >= 0) {
                freezeTimer -= Time.deltaTime;
                return;
            }
            if (prevVelocity != Vector3.zero) {
                GetComponent<Rigidbody>().velocity = prevVelocity;
                prevVelocity = Vector3.zero;
            }
           autoFly();
           launchMissle();
        }
        else {
            if (transform.parent == null)
            {
                transform.parent = GameObject.Find("EnemyManager").transform;
            }
        }
	}


    private void autoFly() {

        Vector3 direction = transform.TransformDirection(Vector3.forward);

        GetComponent<Rigidbody>().velocity = direction * 50f;

        if (flyStatus == -1) {
            transform.LookAt(destination);
            flyStatus = 0;
        }

        if (flyStatus == 0){
            if (transform.position.z < spaceship.transform.position.z - 5) {
                flyStatus = 1;
                transform.LookAt(new Vector3(transform.position.x, transform.position.y, -100));
                StartCoroutine("Rotate");
            }
        } 
        else if (flyStatus == 1) {
            turnAngle += Time.deltaTime * 20;
            transform.Rotate(0, -turnDirection * Time.deltaTime * 20, 0, Space.World);
            if (turnAngle >= 180) {
                StartCoroutine(RotateBack(new Vector3(transform.position.x, Random.Range(-15, 15), 600)));
                flyStatus = 2;
            }
        }
        else if (flyStatus == 2) {
            if (transform.position.z >= 475) {
                flyStatus = 3;
                recomeAngle = Random.Range(5, 10);
                turnningSpeed = Random.Range(20, 30);
                StartCoroutine("Rotate");
            }
        }
        else if (flyStatus == 3) {
            transform.Rotate(0, -turnDirection * Time.deltaTime * turnningSpeed, 0, Space.World);

            if (Mathf.Abs(transform.eulerAngles.y - 180) < recomeAngle)
            {
                StartCoroutine(RotateBack(destination));
                flyStatus = 0;
                turnAngle = 0;
            }
        }
    }

    private void launchMissle() {
        skillTimer -= Time.deltaTime;

        if(flyStatus != -1 && flyStatus != 0 && flyStatus != 3)
            return;

        if (skillTimer <= 0 && transform.position.z > spaceship.transform.position.z + 50)
        {
            GameObject attack = GameObject.Instantiate(bulletPref, transform.position, Quaternion.identity) as GameObject;

            attack.transform.parent = enemySkillManager.transform;

            EnemySkillMotion esm = attack.GetComponent<EnemySkillMotion>();
            esm.setDamage(damage);

            Vector3 ran = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));

            Vector3 vel = Vector3.Normalize(spaceship.transform.position + ran - transform.position) * Random.Range(80f, 100f);
            esm.setVelocity(vel);

            int target = Random.Range(0, 4);
            esm.setIndex(target);

            attack.GetComponent<AutoMove>().startPos = transform.position;
            attack.GetComponent<AutoMove>().velocity = vel;

            NetworkServer.Spawn(attack);

            skillTimer = attackTime;
        }
    }

    public void Freeze(float t) {
        freezeTimer = t;
        prevVelocity = GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    IEnumerator Rotate() {
        while (turnAngleZ <= 60f) {
            transform.Rotate(0, 0, turnDirection * 0.6f);
            turnAngleZ += 0.6f;
            yield return new WaitForSeconds(0.02f);
        }
        turnAngleZ = 60f;
    }

    IEnumerator RotateBack(Vector3 toLookAt) {
        while (turnAngleZ >= 0)
        {
            transform.Rotate(0, 0, -turnDirection * 0.6f);
            turnAngleZ -= 0.6f;
            yield return new WaitForSeconds(0.02f);
        }
        turnAngleZ = 0;
        transform.LookAt(toLookAt);
    }

}
