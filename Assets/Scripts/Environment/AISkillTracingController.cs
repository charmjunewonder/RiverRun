using UnityEngine;
using System.Collections;

public class AISkillTracingController : MonoBehaviour {

    public GameObject chasee;

    public float speed;


	void Start () {
	
	}
	
	void Update () {

        if (chasee == null)
        {
            Destroy(gameObject);
            return;
        }
            

        Vector3 velocity = (chasee.transform.position - transform.position).normalized * speed;
        transform.position += velocity;

        if (Vector3.Distance(transform.position, chasee.transform.position) < 5f) {
            chasee.GetComponent<AllianceAI>().Explode();
            Destroy(gameObject);
        }

	}
}
