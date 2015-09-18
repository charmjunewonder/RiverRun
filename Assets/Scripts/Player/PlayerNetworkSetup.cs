using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour {

	[SerializeField] GameObject playerCam;
	
	// Use this for initialization
	public override void OnStartLocalPlayer ()
	{
		playerCam.SetActive(true);
	}
}
