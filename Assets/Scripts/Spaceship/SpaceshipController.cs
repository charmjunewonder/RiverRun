using UnityEngine;
using System.Collections;

public class SpaceshipController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject.Find("SceneCamera").SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(0, 0, 0.1f);
	}
}
