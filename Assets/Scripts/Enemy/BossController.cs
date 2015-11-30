using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BossController : EnemyMotion{

    private Vector3 newDes;

	void Start () {
        if (isServer) {
            velocity = -transform.position.normalized;
            skillTimer = 5;
            newDes = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 450);
        }
	}
	
	void Update () {
        if (isServer) {
            if (transform.position.z > 450)
            {
                transform.position += velocity;
                return;
            }

            transform.position += (newDes - transform.position).normalized * 0.2f;

            if (Vector3.Distance(transform.position, newDes) < 1) {
                newDes = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 450);
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

            GameObject attack = GameObject.Instantiate(bulletPref, pos, Quaternion.identity) as GameObject;

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
        
        if (blood <= 0)
        {
            transform.parent.GetComponent<EnemySpawnManager>().AddProgress(2);

            GameObject particle = Instantiate(dieParticlePref, gameObject.transform.position, Quaternion.identity) as GameObject;

            RpcCreateDieParticle();

            Debug.Log("CmdDecreaseBlood " + blood);

            for (int i = 0; i < NetworkManagerCustom.SingletonNM.gameplayerControllers.Count; i++)
            {
                if (NetworkManagerCustom.SingletonNM.gameplayerControllers[i] != null)
                {
                    PlayerController pc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[i];
                    pc.score += ScoreParameter.Boss_Score;
                }
            }

            NetworkManagerCustom.SingletonNM.EndGame();

            NetworkServer.Destroy(gameObject);
        }
    }

    [ClientRpc]
    void RpcCreateDieParticle()
    {
        GameObject particle = Instantiate(dieParticlePref, gameObject.transform.position, Quaternion.identity) as GameObject;
    }
}
