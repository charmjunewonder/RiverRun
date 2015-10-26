using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemySkillMotion : NetworkBehaviour {

    private GameObject spaceship;
    [SyncVar]
    private int target;
    public Vector3 velocity;
    private float damage;

    public void setSpaceship(GameObject ss) { spaceship = ss; }
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
            if (Vector3.Distance(spaceship.transform.position, transform.position) > 10)
            {
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                NetworkManagerCustom.SingletonNM.AttackPlayer(target, damage);
                NetworkServer.Destroy(gameObject);
            }
        }
        else {
            if (transform.parent == null) {
                transform.parent = GameObject.FindWithTag("EnemySkillController").transform;
                
                gameObject.layer = target + 8;
            }
        }
	}

    public void Defended() {
        NetworkServer.Destroy(gameObject);
    }

}
