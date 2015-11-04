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


    public void setSpaceship(GameObject ss) { spaceship = ss; }
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

            velocity = 3*Vector3.Normalize(destination - transform.position) * Random.Range(0.1f, 0.2f);

            skillTimer = Random.Range(3.0f, 15f);

        }

	}
	
	void Update () {
        if (isServer)
        {
            if (Vector3.Distance(destination, transform.position) < 5)
            {
                NetworkServer.Destroy(gameObject);
            }

            transform.position += velocity;

            skillTimer -= Time.deltaTime;

            if (skillTimer <= 0 && transform.position.z > spaceship.transform.position.z  + 50)
            {
                GameObject attack = GameObject.Instantiate(bulletPref, transform.position, Quaternion.identity) as GameObject;

                attack.transform.parent = enemySkillManager.transform;



                EnemySkillMotion esm = attack.GetComponent<EnemySkillMotion>();
                esm.setDamage(1);

                Vector3 ran = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));

                Vector3 vel = Vector3.Normalize(spaceship.transform.position + ran - transform.position) * Random.Range(30f, 40f);
                esm.setVelocity(vel);
                esm.setSpaceship(spaceship);

                int target = Random.Range(0, 4);
                esm.setIndex(target);
               


                attack.GetComponent<AutoMove>().startPos = transform.position;
                attack.GetComponent<AutoMove>().velocity = vel;

                NetworkServer.Spawn(attack);

                skillTimer = Random.Range(5.0f, 20.0f);
            }
        }
        else {
            if (transform.parent == null)
            {
                transform.parent = GameObject.Find("EnemyManager").transform;
            }
        }
	}

}
