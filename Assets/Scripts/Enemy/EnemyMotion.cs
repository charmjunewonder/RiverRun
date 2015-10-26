﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyMotion : NetworkBehaviour {

    public GameObject bullet;
    [SyncVar]
    public int index;

    private Vector3 velocity;
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
            NetworkServer.Destroy(gameObject);
        }
    }

	void Start () {
        if (isServer) {
            spaceship = transform.parent.GetComponent<EnemySpawnManager>().GetSpaceShip();

            velocity = Vector3.Normalize(spaceship.transform.position - transform.position) * Random.Range(0.1f, 0.15f);

            skillTimer = Random.Range(3.0f, 15f);

        }

	}
	
	void Update () {
        if (isServer)
        {
            if (Vector3.Distance(spaceship.transform.position, transform.position) > 300)
            {
                transform.position += velocity;
            }

            skillTimer -= Time.deltaTime;

            if (skillTimer <= 0)
            {
                GameObject attack = GameObject.Instantiate(bullet, transform.position, Quaternion.identity) as GameObject;

                attack.transform.parent = enemySkillManager.transform;



                EnemySkillMotion esm = attack.GetComponent<EnemySkillMotion>();
                esm.setDamage(1);
                Vector3 vel = Vector3.Normalize(spaceship.transform.position - transform.position) * Random.Range(20f, 30f);
                esm.setVelocity(vel);
                esm.setSpaceship(spaceship);

                int target = Random.Range(0, 4);
                Debug.Log("Attack Layer " + target);
                esm.setIndex(target);
                
                attack.layer = 8 + target;


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
