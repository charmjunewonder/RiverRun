using UnityEngine;
using System.Collections;

public class defenderUltimate : MonoBehaviour
{

	public bool triggerUltimate;
	public int crystalNumber;
	public float freezeTime;

    public GameObject leftLightening;
    public GameObject rightLightening;
    public GameObject ball;
    public GameObject ultimate;

	void Start ()
	{
		crystalNumber = 0;
		freezeTime = 8;
		triggerUltimate = false;
	}


	IEnumerator WaitAndTurnOffWave ()
	{
		yield return new WaitForSeconds (3);
        ultimate.transform.GetChild(0).gameObject.SetActive(false);
        ultimate.gameObject.SetActive(false);
	}


    public void TriggerUlti()
    {
        triggerUltimate = true;
        ball.gameObject.SetActive(true);
        leftLightening.SetActive(true);
        rightLightening.SetActive(true);
    }

    public void AddCrystal()
    {
        if (triggerUltimate)
        {
            crystalNumber++;

            ball.GetComponent<ParticleSystem>().startSize = 1 + crystalNumber * 0.3f;
        }
    }

    public void Succeed()
    {
        if (triggerUltimate)
        {
            Debug.Log("Ulti Succeed");
            ultimate.SetActive(true);
            ultimate.transform.GetChild(0).gameObject.SetActive(true);

            StartCoroutine("WaitAndTurnOffWave");
            triggerUltimate = false;
            ball.GetComponent<ParticleSystem>().startSize = 1;
            ball.gameObject.SetActive(false);
            crystalNumber = 0;
            leftLightening.SetActive(false);
            rightLightening.SetActive(false);

        }
    }

    public void Fail()
    {
        if (triggerUltimate)
        {
            Debug.Log("Ulti Fail");
            triggerUltimate = false;
            ball.GetComponent<ParticleSystem>().startSize = 1;
            ball.gameObject.SetActive(false);
            crystalNumber = 0;
            leftLightening.SetActive(false);
            rightLightening.SetActive(false);

        }
    }
}
