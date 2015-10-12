using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum LevelEnum { Unselected, Easy, Medium, Hard };

public class LobbyLevelPanel : MonoBehaviour {

    public GameObject[] levelButtons;
    public Text infoText;
    public LobbyPlayer localLobbyPlayer;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnSelectEasyLevel()
    {
        SendLevelInfo(LevelEnum.Easy);
    }

    public void OnSelectMediumLevel()
    {
        SendLevelInfo(LevelEnum.Medium);
    }

    public void OnSelectHardLevel()
    {
        SendLevelInfo(LevelEnum.Hard);
    }

    public void SendLevelInfo(LevelEnum le)
    {
        if (localLobbyPlayer != null)
        {
            localLobbyPlayer.CmdSetLevelWithSlot(le);
            SetLevelInfo(le);
        }
    }

    public void SetLevelInfo(LevelEnum le)
    {
        infoText.text = "You have selected Level " + le.ToString();
    }
}
