using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemySpawnManager : NetworkBehaviour {
	[SerializeField] GameObject enemyPrefab;
	private GameObject[] enemySpawns;
	private int counter;
	private int numberOfEnemy = 20;
	private int maxNumberOfEnemy = 80;
	private float waveRate = 10f;
	private bool isSpawnActivated = true;
	private GameObject enemies;
	
	public override void OnStartServer ()
	{
		enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
		
		enemies = new GameObject();
		enemies.name = "Enemies";
		
		StartCoroutine(EnemySpawner());

	}
	
	IEnumerator EnemySpawner()
	{
		for(;;)
		{
			GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
			if(zombies.Length < maxNumberOfEnemy)
			{
				CommenceSpawn();
			}
			yield return new WaitForSeconds(waveRate);
			
		}
	}
	
	void CommenceSpawn()
	{
		if(isSpawnActivated)
		{
			for(int i = 0; i < numberOfEnemy; i++)
			{
				int randomIndex = Random.Range(0, enemySpawns.Length);
				SpawnEnemy(enemySpawns[randomIndex].transform.position);
			}
		}
	}
	
	void SpawnEnemy(Vector3 spawnPos)
	{
		counter++;
		GameObject go = GameObject.Instantiate(enemyPrefab, spawnPos, Quaternion.identity) as GameObject;
		go.transform.parent = enemies.transform;
		//go.GetComponent<Zombie_ID>().zombieID = "Zombie " + counter;
		NetworkServer.Spawn(go);
	}
}
