using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum PlayerRole { Striker, Defender, Engineer, Simoner };

public class LobbyPlayer : NetworkBehaviour {

    //OnMyName function will be invoked on clients when server change the value of playerName
    [SyncVar(hook = "OnMyName")]
    public string playerName = "";

    [SyncVar]
    private bool isLevelSelected = false;

    public bool isReady = false;

    [SyncVar]
    public int slot = -1;

    public Canvas lobbyPlayerCanvas;
    public Image lobbyPanel;
    public Text nameText;
    public Button[] rolesButtons;
    public RectTransform panelPos;
    public PlayerRole ownRole;
    public GameObject lobbySelect;
    public Sprite strikerSprite, engineerSprite, defenderSprite;
    public GameObject readyImage;
    [SyncVar]
    public LobbyMode currentLobby;

    void Start()
    {
        panelPos.localPosition = new Vector3(GetLobbyPlayerPos(slot), 85.8f, 0);
        if (isLocalPlayer)
        {
            NetworkManagerCustom.SingletonNM.levelPanel.gameObject.GetComponent<LobbyLevelPanel>().localLobbyPlayer = this;
            Debug.Log("OnServerAddPlayer Lobby " + currentLobby);
            if (currentLobby == LobbyMode.Role)
            {
                NetworkManagerCustom.SingletonNM.ChangeTo(NetworkManagerCustom.SingletonNM.lobbyPanel);
                CmdChangeToLobbyPanel();
            }
        }
    }

    private float GetLobbyPlayerPos(int slot)
    {
        //return -(800 / 2) + 800 / 5 * (slot + 1);
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
        OnMyName(playerName);
    }

    public void OnMyName(string newName)
    {
        playerName = newName;
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
        foreach (Button b in rolesButtons)
        {
            b.interactable = false;
        }
        CmdSetReady();
    }

    #endregion


    [Command]
    public void CmdChooseRole(PlayerRole r)
    {
        //Debug.Log("CmdChooseRole");

        ownRole = r;
        ChooseRole(r);
        RpcChooseRole(r);

    }

    [ClientRpc]
    public void RpcChooseRole(PlayerRole r)
    {
        //Debug.Log("RpcChooseRole");
        ownRole = r;
        ChooseRole(r);
        //OnGUIReady();
        if (isLocalPlayer)
        {
            //var lobby = GuiLobbyManager.singleton as GuiLobbyManager;
            //lobby.ownRole = Roles.Striker;
        }
    }

    [Command]
    public void CmdChangeToLobbyPanel()
    {
        ToggleVisibility(true);
        //RpcChangeToLobbyPanel();
        RpcChangeToLobby();
    }

    [ClientRpc]
    public void RpcChangeToLobbyPanel()
    {
        ToggleVisibility(true);
    }

    [ClientRpc]
    public void RpcChangeToLobby()
    {
        if (isLocalPlayer)
        {
            NetworkManagerCustom.SingletonNM.ChangeToLobbyPanelUtil();
        }
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
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdSetReady()
    {
        isReady = true;
        readyImage.SetActive(true);
        RpcSetReady();
    }

    [ClientRpc]
    public void RpcSetReady()
    {
        readyImage.SetActive(true);
    }

    [ClientRpc]
    public void RpcLevelNotSame()
    {
        if (isLocalPlayer)
        {
            NetworkManagerCustom.SingletonNM.levelPanel.gameObject.GetComponent<LobbyLevelPanel>().ResetButtons();
        }
    }
}
