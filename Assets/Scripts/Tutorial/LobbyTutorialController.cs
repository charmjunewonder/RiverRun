using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Network;

public class LobbyTutorialController : MonoBehaviour {
    public PlayerRole ownRole;
    public Sprite[] roleSprites;
    public Image playRoleImage;
    public LobbyInfoPanel infoPanel;
    public GameObject lobbyPanel;
	// Use this for initialization
	void Start () {
        ownRole = PlayerRole.Unselected;
	}

    public void GoToToturialScene()
    {
        GameObject[] lobbies = GameObject.FindGameObjectsWithTag("LobbyDelete");
        foreach(GameObject go in lobbies){
            Destroy(go);
        }
        Application.LoadLevel("Tutorial");
    }

    public void GoToLobbyScene()
    {
        Application.LoadLevel("Lobby");
    }

    #region UI Handler

    public void OnChooseStriker()
    {
        ownRole = PlayerRole.Striker;
        playRoleImage.sprite = roleSprites[0];
    }

    public void OnChooseDefender()
    {
        ownRole = PlayerRole.Defender;
        playRoleImage.sprite = roleSprites[2];

    }

    public void OnChooseEngineer()
    {
        ownRole = PlayerRole.Engineer;
        playRoleImage.sprite = roleSprites[1];

    }

    public void OnReadyButton()
    {
        if (ownRole == PlayerRole.Unselected)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.DisplayWarning("Please Select the Role First.", null);
            return;
        }
        lobbyPanel.SetActive(false);
        //TODO:
    }

    #endregion
}
