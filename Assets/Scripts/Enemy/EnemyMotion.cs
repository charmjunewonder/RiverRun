using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyMotion : NetworkBehaviour {

    public GameObject bulletPref;
    public GameObject dieParticlePref;

    [SyncVar]
    public int index;

    public Vector3 velocity;
    public Vector3 destination;

    private GameObject spaceship;
    private GameObject enemySkillManager;

    private float skillTimer;

    [SyncVar]
    public float blood;
    [SyncVar]
    private float max_blood;

    [SyncVar]
    private int flyStatus;

    private int turnDirection;
    private int turnTimer;
    private float turnAngle;
    private float finishTurnAngle;
    private float turnningSpeed;
    private float turnAngleZ;

    public float recomeAngle;

    private Vector3 pz = new Vector3(0, 0, 1000);
    private Vector3 nz = new Vector3(0, 0, -1000);


    private void setSpaceship(GameObject ss) { spaceship = ss; }
    public void setEnemySkills(GameObject es) { enemySkillManager = es; }
    public void setBlood(float b) { blood = b; }
    public float getBlood() { return blood; }
    public void setMaxBlood(float b) { max_blood = b; }
    public float getMaxBlood() { return max_blood; }
    public void setIndex(int ind) { index = ind; }


    public void DecreaseBlood(float damage) {
        blood -= damage;
        Debug.Log("CmdDecreaseBlood " + blood);
        if (blood <= 0)
        {
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

            destination = spaceship.transform.position + new Vector3(0, 20, -30);

            transform.LookAt(destination);

            skillTimer = Random.Range(3.0f, 15f);

            turnDirection = Random.Range(0, 2);
            turnDirection = turnDirection == 0 ? -1 : 1;
            flyStatus = -1;
            turnAngle = 0;
            turnTimer = 0;
            turnAngleZ = 0.0f;
        }

	}
	
	void Update () {
        if (isServer)
        {
           autoFly();
           launchMissle();
        }
        else {
            if (transform.parent == null)
            {
                transform.parent = GameObject.Find("EnemyManager").transform;
            }
        }
            /*
        else {
            if (flyStatus == 1) {
                transform.parent = null;
            }
            else
            {
                if (transform.parent == null)
                {
                    transform.parent = GameObject.Find("EnemyManager").transform;
                }
            }


        }
             */
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
                StartCoroutine(RotateBack(new Vector3(transform.position.x, transform.position.y + Random.Range(-50, 50), 300)));
                flyStatus = 2;
            }
        }
        else if (flyStatus == 2) {
            if (transform.position.z >= 550) {
                flyStatus = 3;
                recomeAngle = Random.Range(15, 30);
                turnningSpeed = Random.Range(10, 30);
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

        if(flyStatus != 0 && flyStatus != 3)
            return;

        if (skillTimer <= 0 && transform.position.z > spaceship.transform.position.z + 50)
        {
            GameObject attack = GameObject.Instantiate(bulletPref, transform.position, Quaternion.identity) as GameObject;

            attack.transform.parent = enemySkillManager.transform;

            EnemySkillMotion esm = attack.GetComponent<EnemySkillMotion>();
            esm.setDamage(1);

            Vector3 ran = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));

            Vector3 vel = Vector3.Normalize(spaceship.transform.position + ran - transform.position) * Random.Range(80f, 100f);
            esm.setVelocity(vel);
            esm.setSpaceship(spaceship);

            int target = Random.Range(0, 4);
            esm.setIndex(target);

            attack.GetComponent<AutoMove>().startPos = transform.position;
            attack.GetComponent<AutoMove>().velocity = vel;

            NetworkServer.Spawn(attack);

            skillTimer = Random.Range(5.0f, 7.0f);
        }
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
