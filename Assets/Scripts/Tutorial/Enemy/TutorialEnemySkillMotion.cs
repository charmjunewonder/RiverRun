using UnityEngine;
using System.Collections;

public class TutorialEnemySkillMotion : MonoBehaviour {

    public Vector3 speed;

    void Start()
    {
        transform.LookAt(new Vector3(0, 0, 0));

        speed = (Vector3.zero - transform.position).normalized * 0.2f;

    }

    void Update()
    {
        transform.position += speed;

    }
}
