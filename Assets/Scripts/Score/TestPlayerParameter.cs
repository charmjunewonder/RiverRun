using UnityEngine;
using System.Collections;

public class TestPlayerParameter : MonoBehaviour {

    public PlayerRole role;
    public int rank;
	// Use this for initialization
	void Start () {
        PlayerParameter player_0;
        player_0 = GetComponent<PlayerParameter>();
        player_0 = player_0.getPlayer(role,rank);
        player_0.displayPlayerData();

        EnemyParameter enemies = GetComponent<EnemyParameter>();
        enemies = enemies.getEnemys(rank);
        enemies.displayEnemyData();
	}
	
	// Update is called once per frame
	void Update () {
      
	}
}
