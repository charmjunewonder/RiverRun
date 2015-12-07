using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class ServerGameController : NetworkBehaviour {
    public GameObject legs;
    public GameObject wholeShipPrefab;

	// Use this for initialization
	void Start () {
        if (isServer)
        {
            legs.SetActive(false);
            Instantiate(wholeShipPrefab, new Vector3(0, -8.5f, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
