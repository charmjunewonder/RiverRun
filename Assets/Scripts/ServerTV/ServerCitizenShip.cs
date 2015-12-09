using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ServerCitizenShip : MonoBehaviour {
    public ServerPlayerHealth hc;
	// Use this for initialization
	void Start () {
        SetHealth(1);
	}

    public void SetHealth(float health)
    {
        hc.setHealth(health);
    }
}
