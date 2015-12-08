using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerGamePanel : MonoBehaviour {
    public ServerPlayerInfo[] playerInfos;
    public ServerCitizenShip citizenShipHealth;
    public Image pauseButton;
    public Sprite[] pauseSprite;
	// Use this for initialization
	void Start () {
	
	}

    public void Reset()
    {
        foreach (ServerPlayerInfo spi in playerInfos)
        {
            if (spi != null)
            {
                spi.Reset();
                spi.gameObject.SetActive(false);
            }
        }
    }

    public void SetCitizenShipHealth(float health)
    {
        citizenShipHealth.SetHealth(health);
    }

    public void OnPauseClicked()
    {
        //Debug.Log("OnPauseClicked");
        if (NetworkManagerCustom.SingletonNM.isPause)
        {
            pauseButton.sprite = pauseSprite[0];
            NetworkManagerCustom.SingletonNM.UnpauseTheGame();
            NetworkManagerCustom.SingletonNM.isPause = false;
        }
        else
        {
            pauseButton.sprite = pauseSprite[1];
            NetworkManagerCustom.SingletonNM.PauseTheGame();
            NetworkManagerCustom.SingletonNM.isPause = true;
        }
    }
}
