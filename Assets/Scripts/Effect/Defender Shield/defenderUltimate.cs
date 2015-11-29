using UnityEngine;
using System.Collections;

public class defenderUltimate : MonoBehaviour
{

	//public GameObject lightningPrefab;
	public bool triggerUltimate;
	public int crystalNumber;
	public bool succeedUltimate;
	public bool failUltimate;
	public float freezeTime;

	private GameObject lightning;
	private GameObject ball;
	private GameObject wave;
	private GameObject net;
	private ParticleSystem ballParticle;
	// Use this for initialization
	void Start ()
	{
		crystalNumber = 0;
		freezeTime = 8;
		triggerUltimate = false;
		succeedUltimate = false;
		failUltimate = false;
		lightning = this.gameObject.transform.GetChild (0).gameObject;
		ball = this.gameObject.transform.GetChild (1).gameObject;
		wave = this.gameObject.transform.GetChild (2).gameObject;
		net = this.gameObject.transform.GetChild (3).gameObject;
		ballParticle = ball.gameObject.GetComponent<ParticleSystem> ();
		ballParticle.startSize = 4;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (triggerUltimate) {
			lightning.gameObject.SetActive (true);
			StartCoroutine ("WaitAndShowBall");
			ballParticle.startSize = crystalNumber * 0.4f + 4;
		} else {
			ball.gameObject.SetActive (false);
			lightning.gameObject.SetActive (false);
		}

		if (succeedUltimate) {
			if (triggerUltimate) {
				wave.gameObject.SetActive (true);
				triggerUltimate = false;
				failUltimate = false;
				succeedUltimate = false;
				StartCoroutine ("WaitAndShootNet");
				StartCoroutine ("WaitAndDefrozen");
				StartCoroutine ("WaitAndTurnOffWave");
				crystalNumber = 0;
			}
		}
		if (failUltimate) {
			if (triggerUltimate) {
				succeedUltimate = false;
				triggerUltimate = false;
				failUltimate = false;
				crystalNumber = 0;
				freezeTime = 8;
			}
		}
	}


	IEnumerator WaitAndShowBall ()
	{
		yield return new WaitForSeconds (0.1f);
		ball.gameObject.SetActive (true);
	}

	IEnumerator WaitAndTurnOffWave ()
	{
		yield return new WaitForSeconds (3);
		wave.gameObject.SetActive (false);
	}
	IEnumerator WaitAndShootNet ()
	{
		yield return new WaitForSeconds (0.3f);
		net.gameObject.SetActive (true);
	}
	IEnumerator WaitAndDefrozen ()
	{
		yield return new WaitForSeconds (freezeTime);
		net.gameObject.SetActive (false);
	}
}
