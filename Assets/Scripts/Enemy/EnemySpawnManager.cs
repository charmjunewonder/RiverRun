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
    
    private int bossBlood;
    private bool isReadyForNewWave;

    void Start() {
        if (isServer) {
            scoreObject = GameObject.FindGameObjectWithTag("DataSource");
            transform.rotation = Quaternion.identity;
            transform.position = new Vector3(0, 0, 0);
            waves = 0;
            currentTime = 0;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            int input_rank = 0;

            if (NetworkManagerCustom.SingletonNM.selectedDifficulty == LevelEnum.Easy)
            {
                int a = 1000;
                foreach (GameObject player in players)
                {
                    if(player != null)
                        a = Mathf.Min(player.GetComponent<PlayerController>().rank, a); 
                }
                input_rank = a;
            }
            else if (NetworkManagerCustom.SingletonNM.selectedDifficulty == LevelEnum.Medium) {
                int count = 0;
                foreach (GameObject player in players)
                {
                    if (player != null)
                    {
                        input_rank += player.GetComponent<PlayerController>().rank;
                        count++;
                    }
                        
                }
                input_rank /= count;
            }
            else {
                int a = 0;
                foreach (GameObject player in players)
                {
                    if(player != null)
                        a = Mathf.Max(player.GetComponent<PlayerController>().rank, a);
                }
                input_rank = a;
            }

            Debug.Log("input+rank " + input_rank);

            enemyParameter = scoreObject.GetComponent<EnemyParameter>().getEnemys(input_rank);

            bossBlood = (input_rank + 5) * 120;

            max_wave = enemyParameter.enemyWave;
            enemyNumbers = enemyParameter.enemyNumbers;
            enemyData = enemyParameter.enemyDatas;

            maxEnemyNum = 0;
            curEnemyNum = 0;
            for(int i = 0; i < max_wave; i++) {
                maxEnemyNum += enemyNumbers[i];
            }

            
            NetworkManagerCustom.SingletonNM.SetCurrentEnemyWave(0);

            StartCoroutine("PlayingTimeCountDown");

            isReadyForNewWave = true;
            //StartCoroutine(GenerateEnemyInGroup(enemyNumbers[waves]));

        }
    }

    void Update() {
        if (isServer) {
            if (waves > max_wave) return;
            
            if (isReadyForNewWave)
            {
                if (waves >= max_wave)
                {
                    isReadyForNewWave = false;
                    GenerateBoss();
                    waves++;
                }
                else {
                    if (transform.childCount <= 2)
                    {
                        isReadyForNewWave = false;

                        StartCoroutine(GenerateEnemyInGroup(enemyNumbers[waves]));
                    }       
                }
            }
        }
    }

    public GameObject GetSpaceShip() { return spaceship; }

    public void AddProgress(int num)
    {
        if (num == 1)
        {
            curEnemyNum++;
            NetworkManagerCustom.SingletonNM.AddProgress(((float)curEnemyNum) / maxEnemyNum * 0.8f);
        }
        else
        {
            NetworkManagerCustom.SingletonNM.AddProgress(1.0f);
        }
    }

    private void GenerateEnemies(int enemiesNum, int start){

        if(spaceship == null)
            spaceship = GameObject.Find("Spaceship(Clone)");


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
            int awaves = Mathf.Clamp(waves, 0, max_wave - 1);
            int max_index = start + i < enemyData[awaves].Length ? start + i : enemyData[awaves].Length - 1;

            em.setBlood(enemyData[awaves][max_index].maxHp);
            em.setMaxBlood(enemyData[awaves][max_index].maxHp);
            em.setIndex(start + i);
            em.setDamage(enemyData[awaves][max_index].attackPt);
            em.setAttackTime(enemyData[awaves][max_index].attackTime);

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
        float attacktime = 5 - (int)NetworkManagerCustom.SingletonNM.selectedDifficulty + 1;
        bc.setAttackTime(attacktime);
        bc.setBlood(bossBlood);
        bc.setMaxBlood(bossBlood);
        int bossDamage = 1 + (int)NetworkManagerCustom.SingletonNM.selectedDifficulty;
        bc.setDamage(bossDamage);

        NetworkServer.Spawn(boss);

        AudioController.Singleton.PlayBossComing();
    }

    public void Freeze(float t) {
        countDown += t;

        for (int i = 0; i < transform.childCount; i++){
            Transform tran = transform.GetChild(i);

            if (tran.tag == "Enemy")
                tran.GetComponent<EnemyMotion>().Freeze(t);
            else
                tran.GetComponent<BossController>().Freeze(t);

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

    IEnumerator GenerateEnemyInGroup(int num) {

        yield return new WaitForSeconds(5.0f);

        NetworkManagerCustom.SingletonNM.SetTotalEnemyWave(max_wave);
        NetworkManagerCustom.SingletonNM.SetCurrentEnemyWave(waves+1);

        int start = 0;

        while (num > 0) {
            Debug.Log("waves of numbner coroutine " + waves + " " + num);

            int n = Random.Range(5, 8);

            if(n > num)
                n = num;

            GenerateEnemies(n, start);

            start += n;

            num -= n;

            yield return new WaitForSeconds(5.0f);
        }

        isReadyForNewWave = true;
        Debug.Log("waves of numbner coroutine end " + waves + " " + num);
        waves++;
        
                    

    }

}
