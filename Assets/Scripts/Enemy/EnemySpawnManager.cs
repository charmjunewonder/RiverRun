using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemySpawnManager : NetworkBehaviour {
	[SerializeField] GameObject[] enemyPrefab;

    public GameObject bossPrefab;

    public GameObject enemySkills;

    public GameObject scoreObject;

    private GameObject spaceship;

    public int max_wave, waves;
    private int[] enemyNumbers;
    private EnemyData[][] enemyData;

    public int maxEnemyNum;
    public int curEnemyNum;

    private float countDown;

    private EnemyParameter enemyParameter;

    public float totalTime;
    public static int currentTime = 0;
    
    

    void Start() {
        if (isServer) {
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

            enemyParameter = scoreObject.GetComponent<EnemyParameter>().getEnemys(0);

            max_wave = enemyParameter.enemyWave;
            enemyNumbers = enemyParameter.enemyNumbers;
            enemyData = enemyParameter.enemyDatas;

            maxEnemyNum = 0;
            curEnemyNum = 0;
            for(int i = 0; i < max_wave; i++) {
                maxEnemyNum += enemyNumbers[i];
            }

            StartCoroutine("PlayingTimeCountDown");
        }
    }

    void Update() {
        if (isServer) {
            if (waves > max_wave) return;
            
            countDown -= Time.deltaTime;

            if (countDown <= 0)
            {
                if (waves == max_wave)
                {
                    GenerateBoss();
                    countDown = 40.0f;
                    waves++;
                }
                else {
                    if (transform.childCount <= 2) {
                        GenerateEnemies();
                        countDown = 40.0f;
                        waves++;
                    }
                }
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

            float randomZ = Random.Range(600.0f, 650.0f);
            float randomX = Random.Range(-80.0f, 80.0f);
            float randomY = Random.Range(-80.0f, 80.0f);

            Vector3 pos = new Vector3(randomX, randomY, spaceshipPos.z + randomZ);

            GameObject enemy = GameObject.Instantiate(enemyPrefab[enemyNum], pos, Quaternion.identity) as GameObject;

            enemy.transform.LookAt(new Vector3(0, 0, 1));
            enemy.transform.parent = transform;

            EnemyMotion em = enemy.GetComponent<EnemyMotion>();
            em.setEnemySkills(enemySkills);
            /*
            em.setBlood(10);//enemyData[waves][i].maxHp);
            em.setMaxBlood(10);//enemyData[waves][i].maxHp);
            em.setIndex(i);
            em.setDamage(1);//enemyData[waves][i].attackPt);
            
            em.setAttackTime(5);//enemyData[waves][i].attackTime);
            */

            em.setBlood(enemyData[waves][i].maxHp);//;
            em.setMaxBlood(enemyData[waves][i].maxHp);//);
            em.setIndex(i);
            em.setDamage(enemyData[waves][i].attackPt);//;
            em.setAttackTime(enemyData[waves][i].attackTime);//);

            NetworkServer.Spawn(enemy);
            
        }
    }

    private void GenerateBoss() {
        float randomZ = Random.Range(1200f, 1300.0f);
        float randomX = Random.Range(90, 110);
        float randomY = Random.Range(-20.0f, 20.0f);

        Vector3 pos = new Vector3(randomX, randomY, randomZ);

        GameObject boss = GameObject.Instantiate(bossPrefab, pos, Quaternion.identity) as GameObject;

        boss.transform.LookAt(new Vector3(0, 0, 1));
        boss.transform.parent = transform;

        BossController bc = boss.GetComponent<BossController>();
        bc.setEnemySkills(enemySkills);

        bc.setAttackTime(5);
        bc.setBlood(100);
        bc.setMaxBlood(100);
        bc.setDamage(2);

        NetworkServer.Spawn(boss);
    }

    public void Freeze(float t) {
        countDown += t;
        for (int i = 0; i < transform.childCount; i++){
            Transform tran = transform.GetChild(i);
            tran.GetComponent<EnemyMotion>().Freeze(t);

        }
    }

    IEnumerator PlayingTimeCountDown() {
        while (totalTime >= currentTime)
        {
            currentTime += 1;
            yield return new WaitForSeconds(1);
        }

        NetworkManagerCustom.SingletonNM.EndGame();
    }

    public void AddProgress(int num) {
        if (num == 1) {
            curEnemyNum++;
            NetworkManagerCustom.SingletonNM.AddProgress(((float)curEnemyNum) / maxEnemyNum * 0.8f);
        }
        else {
            NetworkManagerCustom.SingletonNM.AddProgress(1.0f);
        }
    }

}
