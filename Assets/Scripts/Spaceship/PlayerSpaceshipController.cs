using UnityEngine;
using System.Collections;

public class PlayerSpaceshipController : MonoBehaviour {
    public GameObject[] citizenShipDamage;
    //public int citizenshipHealth = 10;
    public GameObject[] playerSpaceshipDamage;
    //public float playerSpaceshipHealth = 40f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //SetCitizenShipDamage(citizenshipHealth);
        //SetPlayerSpaceshipDamage(playerSpaceshipHealth);
	}

    public void SetCitizenShipDamage(float damage)
    {
        if (damage > 5 || damage < 0)
        {
            for (int i = 0; i < 5; i++)
            {
                citizenShipDamage[i].SetActive(false);
            }
            return;
        }
        int num = (int)(5 - damage);
        for (int i = 0; i < num; i++)
        {
            citizenShipDamage[i].SetActive(true);
        }
        for (int i = num; i < 5; i++)
        {
            citizenShipDamage[i].SetActive(false);
        }
    }

    public void SetPlayerSpaceshipDamage(float damage)
    {
        if (damage > 24 || damage < 0)
        {
            for (int i = 0; i < 6; i++)
            {
                playerSpaceshipDamage[i].SetActive(false);
            }
            return;
        }
        int num = (int)((24 - damage)/4);
        for (int i = 0; i < num; i++)
        {
            playerSpaceshipDamage[i].SetActive(true);
        }
        for (int i = num; i < 6; i++)
        {
            playerSpaceshipDamage[i].SetActive(false);
        }
    }
}
