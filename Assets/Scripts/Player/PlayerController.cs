using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {
	public GameObject attackPrefab;
	[SerializeField] private Camera cam;
	// Use this for initialization
	void Start () {
		if(isLocalPlayer) 
			cam.enabled = true;
		
	}
	
	// Update is called once per frame
	void Update () {		
		if(!isLocalPlayer) return;
		cam.enabled = true;
#if UNITY_IOS
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
#endif
#if UNITY_STANDALONE_WIN
		if (Input.GetMouseButtonDown(0)) {
#endif
			// Check if the GameObject is clicked by casting a
			// Ray from the main camera to the touched position.
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			CmdDoFire(10, ray);
#if UNITY_IOS
			}
#endif
		}
		
	}
	
	[Command]
	void CmdDoFire(float lifeTime, Ray ray)
	{
		GameObject bullet = (GameObject)Instantiate(
			attackPrefab, 
			transform.position + ray.direction * 10,
			Quaternion.identity);
		
		//Destroy(bullet, lifeTime);
		
		NetworkServer.Spawn(bullet);
		bullet.AddComponent<Rigidbody>();
		bullet.GetComponent<Rigidbody>().useGravity = false;
		bullet.GetComponent<Rigidbody>().velocity = ray.direction * 100;
		
	}
	
	
}
