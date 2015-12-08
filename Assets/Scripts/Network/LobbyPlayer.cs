using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum PlayerRole { Striker, Defender, Engineer, Simoner, Unselected };

public class LobbyPlayer : NetworkBehaviour {

    //OnMyName function will be invoked on clients when server change the value of playerName
    [SyncVar(hook = "OnMyRole")]
    public PlayerRole ownRole;
    [SyncVar(hook = "OnMyName")]
    public string userName = "";
    [SyncVar(hook = "OnReadySet")]
    public bool isReady = false;

    [SyncVar]
    public int slot = -1;

    public Canvas lobbyPlayerCanvas;
    public Image lobbyPanel;
    public Text nameText;
    public Button[] rolesButtons;
    public RectTransform panelPos;
    public GameObject lobbySelect;
    public Sprite strikerSprite, engineerSprite, defenderSprite, unclickedSprite;
    public GameObject readyImage;
    public Text strikerLevel, engineerLevel, defenderLevel;


    void Start()
    {
        panelPos.localPosition = new Vector3(GetLobbyPlayerPos(slot), 85.8f, 0);
        if (!userName.Equals(""))
        {
            nameText.text = userName;
        }
        if (isLocalPlayer)
        {
            CmdNameChanged(LoginController.userName);
            ownRole = PlayerRole.Unselected;

            NetworkManagerCustom.SingletonNM.levelPanel.gameObject.GetComponent<LobbyLevelPanel>().localLobbyPlayer = this;
            Debug.Log("OnServerAddPlayer Lobby " );
            //if (currentMode == NetworkMode.Lobby && currentLobby == LobbyMode.Role)
            //{
            //    Debug.Log("Lobby Player Change to " + currentLobby);

            //    NetworkManagerCustom.SingletonNM.ChangeTo(NetworkManagerCustom.SingletonNM.lobbyPanel);
            //    CmdChangeToLobbyPanel();
            //}
            strikerLevel.text = "RANK " + LoginController.StrikerLevel;
            engineerLevel.text = "RANK " + LoginController.EngineerLevel;
            defenderLevel.text = "RANK " + LoginController.DefenderLevel;
            nameText.text = "<color=#79C25DFF>" + userName + "</color>";


        }
        OnMyName(userName);
        OnMyRole(ownRole);
        OnReadySet(isReady);
    }

    private float GetLobbyPlayerPos(int slot)
    {
        //return -(800 / 2) + 800 / 5 * (slot + 1);
        if (slot < 0 || slot > 3) return -215;
        int[] pos = { -215, -81, 55, 189 };
        return pos[slot];
    }

    public void ToggleVisibility(bool visible)
    {
        lobbyPlayerCanvas.enabled = visible;
        if (isLocalPlayer)
        {
            lobbySelect.SetActive(true);
        }
    }

    [Command]
    public void CmdSetLevelWithSlot(LevelEnum le)
    {
        NetworkManagerCustom.SingletonNM.SetLevelWithSlot(le, slot);
    }

    public override void OnStartClient()
    {
        //All networkbehaviour base function don't do anything
        //but NetworkLobbyPlayer redefine OnStartClient, so we need to call it here
        base.OnStartClient();

        //setup the player data on UI. The value are SyncVar so the player
        //will be created with the right value currently on server
        //OnMyName(playerName);
    }

    #region UI Handler

    public void OnChooseStriker()
    {
        CmdChooseRole(PlayerRole.Striker);
    }

    public void OnChooseDefender()
    {
        CmdChooseRole(PlayerRole.Defender);

    }

    public void OnChooseEngineer()
    {
        //Debug.Log("OnChooseEngineer");
        CmdChooseRole(PlayerRole.Engineer);

    }

    public void OnReadyButton()
    {
        if (ownRole == PlayerRole.Unselected)
        {
            NetworkManagerCustom.SingletonNM.ShowWarning("Please Select the Role First.");
            return;
        }
        foreach (Button b in rolesButtons)
        {
            b.interactable = false;
        }
        CmdSetReady();
    }

    #endregion

    public void OnMyName(string newName)
    {
        //Debug.Log("OnMyName " + newName);
        userName = newName;
        nameText.text = newName;
        if(isLocalPlayer)
            nameText.text = "<color=#79C25DFF>" + userName + "</color>";

    }

    public void OnMyRole(PlayerRole newRole)
    {
        //Debug.Log("OnMyRole " + userName + " " + newRole);
        ownRole = newRole;
        ChooseRole(ownRole);
    }

    public void OnReadySet(bool ready){
        //Debug.Log("OnReadySet " + ready);
        isReady = ready;
        readyImage.SetActive(ready);
    }

    [Command]
    public void CmdChooseRole(PlayerRole r)
    {
        //Debug.Log("CmdChooseRole");
        ownRole = r;
        ChooseRole(ownRole);
    }
  
    public void ServerToggleVisibility(bool visible)
    {
        ToggleVisibility(visible);
        RpcToggleVisibility(visible);
    }

    [ClientRpc]
    public void RpcToggleVisibility(bool visible)
    {
        ToggleVisibility(visible);
    }

    private void ChooseRole(PlayerRole r)
    {
        //foreach (Button b in rolesButtons)
        //{
        //    b.gameObject.SetActive(false);
        //}
        switch (r)
        {
            case PlayerRole.Defender:
                lobbyPanel.sprite = defenderSprite;
                break;
            case PlayerRole.Striker:
                lobbyPanel.sprite = strikerSprite;
                break;
            case PlayerRole.Engineer:
                lobbyPanel.sprite = engineerSprite;
                break;
        }
    }

    [Command]
    public void CmdNameChanged(string newName)
    {
        userName = newName;
        nameText.text = newName;
    }

    [ClientRpc]
    public void RpcNameExist()
    {
        if (isLocalPlayer)
        {
            //Debug.Log("RpcNameExist " + userName);
            NetworkManagerCustom.SingletonNM.StopClient();
            NetworkManagerCustom.SingletonNM.ChangeToConnectPanel();
            NetworkManagerCustom.SingletonNM.ShowWarning("This Account Has Already Connected to Server.");
        }
    }

    [Command]
    public void CmdSetReady()
    {
        isReady = true;
        readyImage.SetActive(true);
        //RpcSetReady();
    }

    [ClientRpc]
    public void RpcLevelNotSame()
    {
        if (isLocalPlayer)
        {
            NetworkManagerCustom.SingletonNM.levelPanel.gameObject.GetComponent<LobbyLevelPanel>().LevelSelectNotSame();
            AudioController.Singleton.PlayBadFeedBack();
        }
    }

    public void ResetUI()
    {
        foreach (Button b in rolesButtons)
        {
            b.image.sprite = unclickedSprite;
        }
    }
}
