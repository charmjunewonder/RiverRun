using UnityEngine;
using System.Collections;

public class WakeUpEnemyManager : MonoBehaviour {
    public GameObject enemyManager;

	// Use this for initialization
	void Start () {
        StartCoroutine(CheckEnemyManager());
	}

    IEnumerator CheckEnemyManager()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("CheckEnemyManager");
        enemyManager.SetActive(true);

        //while (true)
        //{
        //    yield return new WaitForSeconds(2f);
        //    Debug.Log("CheckEnemyManager");
        //    if (enemyManager.activeSelf) break;
        //    enemyManager.SetActive(true);
        //}
    }
}
