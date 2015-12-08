using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemySkillMotion : NetworkBehaviour {

    public GameObject explosionPrefab;

    [SyncVar]
    private int target;
    public Vector3 velocity;
    private float damage;
    private Vector3 prevVelocity;

    public void setIndex(int ind) { target = ind; }
    public void setVelocity(Vector3 v) { velocity = v; }
    public void setDamage(float d) { damage = d; }
    public Vector3 getVelocity() { return velocity; }
    public float getDamage() { return damage; }

	void Start () {
        
	}


	void Update () {
        if (isServer)
        {
            if ( transform.position.z > 0)
            {
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                Vector3 pos = transform.position;
                Vector3 expPos = new Vector3(Random.Range(-3f, 3f), Random.Range(0, 1.5f), Random.Range(-14f, 0f)) + pos;

                Instantiate(explosionPrefab, expPos, Quaternion.identity);

                NetworkManagerCustom.SingletonNM.AttackPlayer(target, damage);
                NetworkServer.Destroy(gameObject);
            }
        }
        else {
            if (transform.parent == null) {
                transform.parent = GameObject.FindWithTag("EnemySkillController").transform;
                
                gameObject.layer = target + 8;
                transform.GetChild(0).gameObject.layer = target + 8;
            }
        }
	}

    public void Defended() {
        NetworkServer.Destroy(gameObject);
    }

    public void Pause() {
        prevVelocity = velocity;
        velocity = Vector3.zero;
        GetComponent<AutoMove>().velocity = Vector3.zero;
    }

    public void UnPause() {
        velocity = prevVelocity;
        prevVelocity = Vector3.zero;
        RpcUnPause(velocity);
    }

    [ClientRpc]
    private void RpcUnPause(Vector3 vec) {
        GetComponent<AutoMove>().velocity = vec;
    }


    void OnTriggerEnter(Collider collider) {
        if (isServer) { 
            Defended();
        }
    }

}
