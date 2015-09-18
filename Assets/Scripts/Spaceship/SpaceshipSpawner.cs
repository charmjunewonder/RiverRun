using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpaceshipSpawner : NetworkBehaviour {
	public GameObject spaceshipPrefab;
	
	// Use this for initialization
	void Start () {
		Spawn();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Spawn(){
		if(isServer){
			GameObject ss = Instantiate(spaceshipPrefab);
			NetworkServer.Spawn(ss);
		}
	}
	
}
