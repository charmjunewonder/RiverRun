using UnityEngine;
using System.Collections;

public class strikerUltimate : MonoBehaviour
{
	public bool triggerUltimate;
	public int crystalNumber;
    public GameObject leftLightening;
    public GameObject rightLightening;

	public GameObject ball;
    public GameObject ultimate;
	public ParticleSystem ballInner;
	public ParticleSystem ballouter;
	

	// Use this for initialization
	void Start ()
	{
		crystalNumber = 0;
		triggerUltimate = false;

	}
	
	IEnumerator WaitAndTurnOffUltimate ()
	{
		yield return new WaitForSeconds (3);
        ultimate.SetActive(false);
	}

    public void TriggerUlti() {
        Debug.Log("Trigger Ulti in Particle");
        triggerUltimate = true;
        ball.gameObject.SetActive(true);
        leftLightening.transform.GetChild(0).position = ball.transform.position;
        rightLightening.transform.GetChild(0).position = ball.transform.position;
        Debug.Log("Left lightening position " + leftLightening.transform.GetChild(0).position + " " + ball.transform.position);
        
    }

    public void AddCrystal(bool isServer) {
        if (triggerUltimate)
        {
            crystalNumber++;
            if (isServer) {
                ballInner.emissionRate = crystalNumber * 3 + 5;
                ballInner.startSize = crystalNumber * 1.5f + 3;
                ballouter.emissionRate = crystalNumber * 70 + 100;
                ballouter.startSize = crystalNumber * 0.35f + 0.7f;
                ballouter.gameObject.transform.localScale = new Vector3(3 + crystalNumber, 3 + crystalNumber, 3 + crystalNumber);
            }
            else {
                ballInner.emissionRate = crystalNumber * 3 + 5;
                ballInner.startSize = crystalNumber * 0.5f + 1;
                ballouter.emissionRate = crystalNumber * 70 + 100;
                ballouter.startSize = crystalNumber * 0.15f + 0.7f;
                ballouter.gameObject.transform.localScale = new Vector3(1 + crystalNumber * 0.5f, 1 + crystalNumber * 0.5f, 1 + crystalNumber * 0.5f);
            }
           
        }
    }

    public void Succeed(bool isServer)
    {
        if (triggerUltimate)
        {
            Debug.Log("Ulti Succeed");
            ultimate.SetActive(true);
            ultimate.transform.GetChild(0).gameObject.SetActive(true);
            ultimate.transform.GetChild(1).gameObject.SetActive(true);

            StartCoroutine("WaitAndTurnOffUltimate");
            triggerUltimate = false;
            ball.gameObject.SetActive(false);
            crystalNumber = 0;
            leftLightening.transform.GetChild(0).position = leftLightening.transform.GetChild(1).position;
            rightLightening.transform.GetChild(0).position = rightLightening.transform.GetChild(1).position;

            if (isServer) {
                ballInner.emissionRate = 5;
                ballInner.startSize = 3;
                ballouter.emissionRate = 100;
                ballouter.startSize = 0.7f;
                ballouter.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            else {
                ballInner.emissionRate = 5;
                ballInner.startSize = 1;
                ballouter.emissionRate = 100;
                ballouter.startSize = 0.7f;
                ballouter.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }

       
        }
    }

    public void Fail(bool isServer)
    {
        if (triggerUltimate)
        {
            Debug.Log("Ulti Fail");
            triggerUltimate = false;
            ball.gameObject.SetActive(false);
            crystalNumber = 0;
            leftLightening.transform.GetChild(0).position =  leftLightening.transform.GetChild(1).position;
            rightLightening.transform.GetChild(0).position = rightLightening.transform.GetChild(1).position;

            if (isServer)
            {
                ballInner.emissionRate = 5;
                ballInner.startSize = 3;
                ballouter.emissionRate = 100;
                ballouter.startSize = 0.7f;
                ballouter.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                ballInner.emissionRate = 5;
                ballInner.startSize = 1;
                ballouter.emissionRate = 100;
                ballouter.startSize = 0.7f;
                ballouter.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

}
