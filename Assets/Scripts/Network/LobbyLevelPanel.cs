using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum LevelEnum { Unselected, Easy, Medium, Hard };

public class LobbyLevelPanel : MonoBehaviour {

    public Button[] levelButtons;
    public Text warningText;
    public LobbyPlayer localLobbyPlayer;
    public Sprite unclickedSprite;
    public Text infoText;
	// Use this for initialization

    public void OnSelectEasyLevel()
    {
        SendLevelInfo(LevelEnum.Easy);
        warningText.gameObject.SetActive(false);
        ShowWaitingInfo();
    }

    public void OnSelectMediumLevel()
    {
        SendLevelInfo(LevelEnum.Medium);
        warningText.gameObject.SetActive(false);
        ShowWaitingInfo();
    }

    public void OnSelectHardLevel()
    {
        SendLevelInfo(LevelEnum.Hard);
        warningText.gameObject.SetActive(false);
        ShowWaitingInfo();
    }

    public void SendLevelInfo(LevelEnum le)
    {
        if (localLobbyPlayer != null)
        {
            localLobbyPlayer.CmdSetLevelWithSlot(le);
            //SetLevelInfo(le);
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
        UnshowWaitingInfo();
    }

    public void ShowWaitingInfo()
    {
        StartCoroutine("flashingWait");
    }

    IEnumerator flashingWait()
    {
        bool isRed = false;
        while (true)
        {
            if (isRed)
            {
                infoText.text = @"Please Talk to Your Teammates and Select the <color=#5CB4E1FF><b>Same Difficulty</b></color> as a Team.
<color=#D21D1DFF><b>Wait</b></color> Other Teamates To Select.";
            }
            else
            {
                infoText.text = @"Please Talk to Your Teammates and Select the <color=#5CB4E1FF><b>Same Difficulty</b></color> as a Team.
<color=#FFFFFFFF><b>Wait</b></color> Other Teamates To Select.";
            }
            isRed = !isRed;
            yield return new WaitForSeconds(0.4f);
        }
    }

    public void UnshowWaitingInfo()
    {
        StopCoroutine("flashingWait");
        infoText.text = @"Please Talk to Your Teammates and Select the <color=#5CB4E1FF><b>Same Difficulty</b></color> as a Team.
<color=#D21D1DFF><b>Wait</b></color> Other Teamates To Select.";
    }
}
