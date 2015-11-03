using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum LevelEnum { Unselected, Easy, Medium, Hard };

public class LobbyLevelPanel : MonoBehaviour {

    public Button[] levelButtons;
    public Text warningText;
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
        warningText.gameObject.SetActive(false);
    }

    public void OnSelectMediumLevel()
    {
        SendLevelInfo(LevelEnum.Medium);
        warningText.gameObject.SetActive(false);

    }

    public void OnSelectHardLevel()
    {
        SendLevelInfo(LevelEnum.Hard);
        warningText.gameObject.SetActive(false);

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
        warningText.text = "You have selected Level " + le.ToString();
    }

    public void LevelSelectNotSame()
    {
        ResetButtons();
        warningText.gameObject.SetActive(true);
    }

    public void ResetButtons()
    {
        foreach (Button b in levelButtons)
        {
            b.image.sprite = unclickedSprite;
        }
    }
}
