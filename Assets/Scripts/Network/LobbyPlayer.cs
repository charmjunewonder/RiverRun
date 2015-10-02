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

    public Text nameText;
    public Button[] rolesButtons;
    public Text roleText;
    public Button reselectButton;
    public RectTransform panelPos;

    private PlayerRole ownRole;

    void Start()
    {
        panelPos.localPosition = new Vector3(GetLobbyPlayerPos(slot), 0, 0);
    }

    private float GetLobbyPlayerPos(int slot)
    {
        //return -(800 / 2) + 800 / 5 * (slot + 1);
        return -400 + 160 * (slot + 1);
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
        CmdChooseRole(PlayerRole.Engineer);

    }

    public void OnChooseSimoner()
    {
        CmdChooseRole(PlayerRole.Simoner);

    }
    #endregion


    [Command]
    public void CmdChooseRole(PlayerRole r)
    {
        ownRole = r;
        ChooseRole(r);
        RpcChooseRole(r);
        SetReady();
    }

    [ClientRpc]
    public void RpcChooseRole(PlayerRole r)
    {
        ownRole = PlayerRole.Striker;
        ChooseRole(r);
        //OnGUIReady();
        if (isLocalPlayer)
        {
            //var lobby = GuiLobbyManager.singleton as GuiLobbyManager;
            //lobby.ownRole = Roles.Striker;
        }
    }

    private void ChooseRole(PlayerRole r)
    {
        foreach (Button b in rolesButtons)
        {
            b.gameObject.SetActive(false);
        }
        roleText.gameObject.SetActive(true);
        roleText.text = r.ToString();
    }

    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }

    
    private void SetReady()
    {
        isReady = true;
    }
}
