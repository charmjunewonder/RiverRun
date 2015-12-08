using UnityEngine;
using System.Collections;

public class EnemyAttackFreezer : MonoBehaviour {

    public void Freeze() {
        for(int i = transform.childCount - 1; i >= 0; i--){
            transform.GetChild(i).GetComponent<EnemySkillMotion>().Defended();
        }
    }

    public void Pause() {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            transform.GetChild(i).GetComponent<EnemySkillMotion>().Pause();
        }
    }

    public void UnPause() {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            transform.GetChild(i).GetComponent<EnemySkillMotion>().UnPause();
        }
    }

}
