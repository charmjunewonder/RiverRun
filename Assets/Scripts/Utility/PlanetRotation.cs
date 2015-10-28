using UnityEngine;
using System.Collections;

public class PlanetRotation : MonoBehaviour {

	void Start () {
	
	}
	
	void Update () {
        transform.Rotate(Time.deltaTime * 0.2f, Time.deltaTime * 0.3f, 0);
	}
}
