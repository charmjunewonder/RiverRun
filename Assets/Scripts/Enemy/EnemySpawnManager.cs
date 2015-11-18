using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemySpawnManager : NetworkBehaviour {
	[SerializeField] GameObject[] enemyPrefab;

    public GameObject portalPrefab;

    public GameObject enemySkills;

    public GameObject scoreObject;

    private GameObject spaceship;

    public int max_wave, waves;
    private int[] enemyNumbers;
    private EnemyData[][] enemyData;

    private float countDown;

    private EnemyParameter enemyParameter;

    public float totalTime;

    void Start() {
        if (isServer) {
            scoreObject = GameObject.FindGameObjectWithTag("DataSource");

            transform.rotation = Quaternion.identity;
            transform.position = new Vector3(0, 0, 0);
            waves = 0;
            countDown = 5.0f;

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            int average_rank = 0;

            foreach (GameObject player in players) {
                average_rank += player.GetComponent<PlayerController>().rank;
            }

            average_rank /= players.Length;

            enemyParameter = scoreObject.GetComponent<EnemyParameter>().getEnemys(average_rank);

            max_wave = enemyParameter.enemyWave;
            enemyNumbers = enemyParameter.enemyNumbers;
            enemyData = enemyParameter.enemyDatas;

            StartCoroutine("PlayingTimeCountDown");
        }
    }

    void Update() {
        if (isServer) {
            if (waves >= max_wave) return;
            countDown -= Time.deltaTime;

            if (countDown <= 0)
            {
                GenerateEnemies();
                countDown = 60.0f;
                waves++;
            }
        }
    }

    public GameObject GetSpaceShip() { return spaceship; }

    private void GenerateEnemies() {

        if(spaceship == null)
            spaceship = GameObject.Find("Spaceship(Clone)");

        int enemiesNum = enemyNumbers[waves];



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
            em.setBlood(10);//enemyData[waves][i].maxHp);
            em.setMaxBlood(10);//enemyData[waves][i].maxHp);
            em.setIndex(i);
            em.setDamage(1);//enemyData[waves][i].attackPt);
            em.setAttackTime(5);//enemyData[waves][i].attackTime);

            NetworkServer.Spawn(enemy);
            
        }
    }

    IEnumerator PlayingTimeCountDown() {
        while (totalTime >= 0) {
            totalTime -= 1;
            yield return new WaitForSeconds(1);
        }

        NetworkManagerCustom.SingletonNM.GameEnded();
    }

}
