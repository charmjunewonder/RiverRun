using UnityEngine;
using System.Collections;

public class TutorialGenerateAttack : MonoBehaviour {

    public GameObject enemyManager;
    public GameObject attackPrefab;

    public void GenerateAttack() {
        for (int i = 0; i < enemyManager.transform.childCount; i++) {
            GameObject attack = Instantiate(attackPrefab, enemyManager.transform.GetChild(i).position, Quaternion.identity) as GameObject;
            attack.transform.parent = transform;
        }
    }

}
