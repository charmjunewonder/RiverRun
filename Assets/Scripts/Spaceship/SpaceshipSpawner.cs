﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpaceshipSpawner : NetworkBehaviour {
	public GameObject spaceshipPrefab;
    public GameObject ultiController;

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
            Instantiate(ultiController);
		}
	}
	
}
