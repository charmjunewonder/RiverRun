﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BossController : NetworkBehaviour {

    public GameObject attackPrefab;
    public GameObject dieParticlePref;

    private Vector3 velocity;
    private GameObject enemySkillManager;

    [SyncVar]
    public float blood;
    [SyncVar]
    private float max_blood;

    private int damage;
    public float attackTime;

    public float freezeTimer;
    public Vector3 prevVelocity;

    public float skillTimer;

    public void setEnemySkills(GameObject es) { enemySkillManager = es; }
    public void setBlood(float b) { blood = b; }
    public float getBlood() { return blood; }
    public void setMaxBlood(float b) { max_blood = b; }
    public float getMaxBlood() { return max_blood; }
    public void setDamage(int d) { damage = d; }
    public void setAttackTime(float t) { attackTime = t; skillTimer = attackTime; }

	void Start () {
        if (isServer) {
            velocity = -transform.position.normalized;
            skillTimer = 5;
        }
	}
	
	void Update () {
        if (isServer) {
            if (transform.position.z > 450)
            {
                transform.position += velocity;
                return;
            }

            skillTimer -= Time.deltaTime;
            if (skillTimer <= 0)
            {
                GenerateAttack();
                skillTimer = attackTime;
            }
        }
        else
        {
            if (transform.parent == null)
            {
                transform.parent = GameObject.Find("EnemyManager").transform;
            }
        }
	}

    private void GenerateAttack() {
        for (int i = 0; i < 12; i++) {

            Vector3 pos = transform.position + 20 * (new Vector3(Mathf.Sin(i * Mathf.PI / 6), Mathf.Cos(i * Mathf.PI / 6), 0));

            GameObject attack = GameObject.Instantiate(attackPrefab, pos, Quaternion.identity) as GameObject;

            attack.transform.parent = enemySkillManager.transform;

            EnemySkillMotion esm = attack.GetComponent<EnemySkillMotion>();
            esm.setDamage(damage);

            Vector3 ran = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));

            Vector3 vel = Vector3.Normalize(ran - pos) * Random.Range(80f, 100f);
            esm.setVelocity(vel);

            int target = Random.Range(0, 4);
            esm.setIndex(target);

            attack.GetComponent<AutoMove>().startPos = pos;
            attack.GetComponent<AutoMove>().velocity = vel;

            NetworkServer.Spawn(attack);
        
        }
    }

    public void Freeze(float t) {
        skillTimer += t;
    }

    public void DecreaseBlood(float damage)
    {
        blood -= damage;
        Debug.Log("CmdDecreaseBlood " + blood);
        if (blood <= 0)
        {
            GameObject particle = Instantiate(dieParticlePref, gameObject.transform.position, Quaternion.identity) as GameObject;

            RpcCreateDieParticle();

            NetworkManagerCustom.SingletonNM.GameEnded();

            NetworkServer.Destroy(gameObject);
        }
    }

    [ClientRpc]
    void RpcCreateDieParticle()
    {
        GameObject particle = Instantiate(dieParticlePref, gameObject.transform.position, Quaternion.identity) as GameObject;
    }
}
