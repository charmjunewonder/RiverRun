using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemySpawnManager : NetworkBehaviour {
	[SerializeField] GameObject[] enemyPrefab;

    public int enemiesMin, enemiesMax;

    public GameObject enemySkills;

    private GameObject[] enemies;

    private GameObject spaceship;

    private GameObject[] players;

    void Start() {
        if (isServer) {
            spaceship = GameObject.FindGameObjectWithTag("Spaceship");
            players = GameObject.FindGameObjectsWithTag("Player");
            GenerateEnemies();
        }
    }

    public GameObject GetSpaceShip() { return spaceship; }

    private void GenerateEnemies() {
        int enemiesNum = Random.Range(enemiesMin, enemiesMax);

        for (int i = 0; i < enemiesNum; i++){
            int enemyNum = Random.Range(0, enemyPrefab.Length);

            Vector3 spaceshipPos = spaceship.transform.position;

            float randomZ = Random.Range(550.0f, 600.0f);
            float randomX = Random.Range(-randomZ * 0.4f, randomZ * 0.4f);
            float randomY = Random.Range(-randomZ * 0.4f, randomZ * 0.3f);

            Vector3 pos = new Vector3(randomX, randomY, spaceshipPos.z + randomZ);

            GameObject enemy = GameObject.Instantiate(enemyPrefab[enemyNum], pos, Quaternion.identity) as GameObject;

            enemy.transform.LookAt(new Vector3(0, 0, 1));
            enemy.transform.parent = transform;

            EnemyMotion em = enemy.GetComponent<EnemyMotion>();
            em.setEnemySkills(enemySkills);
            em.setBlood(10.0f);
            em.setMaxBlood(10.0f);
            em.setIndex(i);

            NetworkServer.Spawn(enemy);
            
        }
    }


}
