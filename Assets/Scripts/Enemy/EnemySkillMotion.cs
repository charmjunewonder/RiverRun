using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemySkillMotion : NetworkBehaviour {

    private GameObject spaceship;
    private GameObject targetPlayer;
    private Vector3 velocity;
    private float damage;

    public void setSpaceship(GameObject ss) { spaceship = ss; }
    
    public void setVelocity(Vector3 v) { velocity = v; }
    public void setDamage(float d) { damage = d; }
    public void setTargetPlayer(GameObject tp) { targetPlayer = tp; }
    
    public Vector3 getVelocity() { return velocity; }
    public float getDamage() { return damage; }

	void Start () {
        
	}


	void Update () {
        if (isServer)
        {
            if (Vector3.Distance(spaceship.transform.position, transform.position) > 10)
            {
                transform.position += velocity;
            }
            else
            {
                if(targetPlayer != null)
                    targetPlayer.GetComponent<PlayerController>().RpcDamage(damage);
                NetworkServer.Destroy(gameObject);
            }
        }
        else {
            if (transform.parent == null) {
                transform.parent = GameObject.FindWithTag("EnemySkillController").transform;
            }
        }
	}

    public void Defended() {
        NetworkServer.Destroy(gameObject);
    }

}
