using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum LevelEnum { Unselected, Easy, Medium, Hard };

public class LobbyLevelPanel : MonoBehaviour {

    public Button[] levelButtons;
    public Text infoText;
    public LobbyPlayer localLobbyPlayer;
    public Sprite unclickedSprite;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnSelectEasyLevel()
    {
        SendLevelInfo(LevelEnum.Easy);
        infoText.text = "";
    }

    public void OnSelectMediumLevel()
    {
        SendLevelInfo(LevelEnum.Medium);
        infoText.text = "";

    }

    public void OnSelectHardLevel()
    {
        SendLevelInfo(LevelEnum.Hard);
        infoText.text = "";

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

    public void ResetButtons()
    {
        foreach(Button b in levelButtons)
        {
            b.image.sprite = unclickedSprite;
        }
        infoText.text = "Please select the same level with your teammates!";
    }
}
