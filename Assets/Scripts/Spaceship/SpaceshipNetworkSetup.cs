using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpaceshipNetworkSetup : NetworkBehaviour {
	
	[SerializeField] Camera mainCam;
	
	// Use this for initialization
	public override void OnStartLocalPlayer ()
	{
		GameObject.Find("SceneCamera").SetActive(false);
	}
}
