using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Network;

public enum NetworkMode { Lobby, Game };
public enum LobbyMode { Level, Role };

public class NetworkManagerCustom : NetworkManager {
    public GameObject LobbyPlayerPrefab;
    public GameObject StrikerPrefab;
    public GameObject EngineerPrefab;
    public GameObject DisconnectedPlayerPrefab;

    public ArrayList gameplayerControllers { get; set; }
    public ArrayList lobbyPlayerArray { get; set; }
    public ArrayList disconnectedPlayerControllers { get; set; }
    public NetworkMode currentMode { get; set; }
    public LobbyMode currentLobby;

    static public NetworkManagerCustom SingletonNM { get; set;}
    
    #region Lobby Variable

    public int maxPlayers = 4;
    public int minPlayers = 1;

    public LobbyConnectPanel topPanel;

    public RectTransform connectPanel;
    public RectTransform levelPanel;
    public RectTransform lobbyPanel;
    public RectTransform settingPanel;

    public LobbyInfoPanel infoPanel;

    protected RectTransform currentPanel;
    protected RectTransform previousPanel;

    #endregion

    private string selectedLevel = "Level1";
    private ArrayList levels;
    private bool isServer = false;
    void Start()
    {
        SingletonNM = this;
        currentMode = NetworkMode.Lobby;
        currentLobby = LobbyMode.Level;

        lobbyPlayerArray = new ArrayList(maxPlayers);
        gameplayerControllers = new ArrayList(maxPlayers);
        disconnectedPlayerControllers = new ArrayList(maxPlayers);
        levels = new ArrayList(maxPlayers);
        for (int i = 0; i < maxPlayers; i++)
        {
            lobbyPlayerArray.Add(null);
            gameplayerControllers.Add(null);
            disconnectedPlayerControllers.Add(null);
            levels.Add(LevelEnum.Unselected);
        }

        lobbySystemStartSetting();
    }

    #region Lobby
    private void lobbySystemStartSetting()
    {
        //SetServerInfo("Offline", "None");
        currentPanel = connectPanel;
    }


    public void ChangeTo(RectTransform newPanel)
    {
        if (currentPanel != null)
        {
            currentPanel.gameObject.SetActive(false);
            previousPanel = currentPanel;
        }

        if (newPanel != null)
        {
            newPanel.gameObject.SetActive(true);
        }

        currentPanel = newPanel;

        if (currentPanel == connectPanel)
        {
            //SetServerInfo("Offline", "None");
        }
    }

    public void DisplayIsConnecting()
    {
        var _this = this;
        infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
    }

    #region Disconnect Button
    public delegate void BackButtonDelegate();
    public BackButtonDelegate backDelegate;
    public void GoBackButton()
    {
        backDelegate();
    }

    public void OnSimpleBackClbk()
    {
        Debug.Log("SimpleBackClbk");
        if (previousPanel != null)
            ChangeTo(previousPanel);
    }

    public void StopHostClbk()
    {
        Debug.Log("StopHostClbk");
        StopHost();
        ResetAfterStopServer();
        ChangeTo(connectPanel);
    }

    public void StopClientClbk()
    {
        Debug.Log("StopClientClbk");
        StopClient();
        ChangeTo(connectPanel);
    }

    public void StopServerClbk()
    {
        Debug.Log("StopServerClbk");
        StopServer();
        ResetAfterStopServer();
        ChangeTo(connectPanel);
    }

    public void StopGameClbk()
    {
        Debug.Log("StopGameClbk");
        //SendReturnToLobby();
        ChangeTo(lobbyPanel);
    }

    public void OnLobbyBackToLevelClbk()
    {
        Debug.Log("LobbyBackToLevelClbk");
        if(isServer)
            backDelegate = StopServerClbk;
        else
            backDelegate = StopClientClbk;

        ChangeVisibilityOfLobbyPlayer(false);
        ChangeTo(levelPanel);
    }

    #endregion

    public void ChangeToSettingPanel()
    {
        ChangeTo(settingPanel);
        //backDelegate = SimpleBackClbk;
    }

    IEnumerator CheckLobbyReady()
    {
        Debug.Log("CheckLobbyReady");
        while (true)
        {
            Debug.Log("CheckLobbyReady inside");

            int count = 0;
            bool isAllReady = true;
            for (int i = 0; i < maxPlayers; i++)
            {
                if (lobbyPlayerArray[i] != null)
                {
                    count++;
                    LobbyPlayer lp = (LobbyPlayer)lobbyPlayerArray[i];
                    isAllReady &= lp.isReady;
                }
            }
            if (count >= minPlayers && isAllReady)
            {
                Debug.Log("CheckLobbyReady ready");

                ChangeLobbyToGameScene();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator CheckLevelSelect()
    {
        while (true)
        {
            Debug.Log("CheckLevelSelect");

            LevelEnum firstLevel = LevelEnum.Easy;
            bool isAllSame = false;
            bool isFirst = true;
            bool isAllSelected = false;
            int userCount = 0;
            for (int i = 0; i < maxPlayers; i++)
            {
                if (lobbyPlayerArray[i] != null)
                {
                    LevelEnum curr = (LevelEnum)levels[i];
                    userCount++;
                    if (isFirst) {
                        isAllSame = true;
                        isAllSelected = true;
                        firstLevel = curr;
                        Debug.Log("Level Select Same  " + firstLevel.ToString());
                        isFirst = false;
                        isAllSame &= (firstLevel != LevelEnum.Unselected);
                        isAllSelected &= (firstLevel != LevelEnum.Unselected);
                    }
                    else
                    {
                        isAllSame &= (firstLevel == curr);
                        isAllSelected &= (curr != LevelEnum.Unselected);
                    }
                }
            }
            if(userCount < minPlayers)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }
            if (isAllSame)
            {
                Debug.Log("Level Select Same");
                StartCoroutine("CheckLobbyReady");
                ChangeToLobbyPanel();
            }
            else if(isAllSelected)
            {
                Debug.Log("Level Select Not Same");
                foreach(LobbyPlayer lp in lobbyPlayerArray)
                {
                    if (lp != null)
                    {
                        lp.RpcLevelNotSame();
                    }
                }
                for(int i = 0; i < maxPlayers; i++)
                {
                    levels[i] = LevelEnum.Unselected;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void ChangeToLobbyPanel()
    {
        StopCoroutine("CheckLevelSelect");
        ChangeToLobbyPanelUtil();
        //backDelegate = OnLobbyBackToLevelClbk;
        foreach (LobbyPlayer lp in lobbyPlayerArray)
        {
            if (lp != null)
            {
                lp.RpcChangeToLobby();
            }
        }
    }

    public void ChangeToLobbyPanelUtil()
    {
        ChangeTo(lobbyPanel);
        currentLobby = LobbyMode.Role;
        ChangeVisibilityOfLobbyPlayer(true);
    }

    private void ChangeLobbyToGameScene()
    {
        Debug.Log("ChangeLobbyToGameScene");
        StopCoroutine("CheckLobbyReady");
        DisableLobbyUI();
        //RpcSetupGameScene();
        for (int i = 0; i < maxPlayers; i++)
        {
            if (lobbyPlayerArray[i] != null)
            {
                LobbyPlayer lp = (LobbyPlayer)lobbyPlayerArray[i];
                NetworkConnection conn = lp.connectionToClient;
                if (lp.ownRole == PlayerRole.Engineer){

                    GameObject newPlayer = (GameObject)Instantiate(EngineerPrefab, Vector3.zero, Quaternion.identity);

                    Debug.Log("@@@@@@@@");

                    newPlayer.GetComponent<EngineerController>().slot = lp.slot;
                    gameplayerControllers[i] = newPlayer.GetComponent<EngineerController>();
                    ////NetworkServer.Spawn(newPlayer);
                    short id = lp.GetComponent<LobbyPlayer>().playerControllerId;
                    ////Debug.Log("playercontrollerid: " + id);
                    newPlayer.GetComponent<EngineerController>().role = lp.ownRole;
                    NetworkServer.Destroy(lp.gameObject);

                    ////Destroy(lp.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, newPlayer, id);
                    //Debug.Log("ReplacePlayerForConnection " + success);
                }
                else {
                    GameObject newPlayer = (GameObject)Instantiate(StrikerPrefab, Vector3.zero, Quaternion.identity);
                    newPlayer.GetComponent<PlayerController>().slot = lp.slot;
                    gameplayerControllers[i] = newPlayer.GetComponent<PlayerController>();
                    //NetworkServer.Spawn(newPlayer);
                    short id = lp.GetComponent<LobbyPlayer>().playerControllerId;
                    //Debug.Log("playercontrollerid: " + id);
                    newPlayer.GetComponent<PlayerController>().role = lp.ownRole;
                    NetworkServer.Destroy(lp.gameObject);

                    //Destroy(lp.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, newPlayer, id);
                    //Debug.Log("ReplacePlayerForConnection " + success);
                }
                

            }
        }
        currentMode = NetworkMode.Game;
        ResetLobbyPlayerArray();
        ServerChangeScene(selectedLevel);
    }

    private void SetupGameScene()
    {
        DisableLobbyUI();
    }

    private void DisableLobbyUI()
    {
        topPanel.gameObject.SetActive(false);
        connectPanel.gameObject.SetActive(false);
        lobbyPanel.gameObject.SetActive(false);
        infoPanel.gameObject.SetActive(false);
    }

    public void SetLevelWithSlot(LevelEnum le, int newSlot)
    {
        if(newSlot >=0 && newSlot < maxPlayers)
        {
            Debug.Log("Set Level " + le.ToString() + " " + newSlot);
            levels[newSlot] = le;
        }
    }
    #endregion

    #region ServerOverride
    //This hook is invoked when a server is started - including when a host is started.
    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
        isServer = true;
        currentMode = NetworkMode.Lobby;
        StartCoroutine("CheckLevelSelect");
        if(currentPanel == connectPanel)
            ChangeTo(lobbyPanel);

        //StartCoroutine("CheckLobbyReady");
    }

    //This hook is called when a server is stopped - including when a host is stopped.
    public override void OnStopServer()
    {
        Debug.Log("OnStopServer");
        currentMode = NetworkMode.Lobby;
        StopCoroutine("CheckLobbyReady");
        ResetLobbyPlayerArray();
    }

    //Called on the server when a new client connects.
    public override void OnServerConnect(NetworkConnection conn){
        Debug.Log("OnServerConnect " + conn.connectionId);
        int i = 0;
		for(; i < maxPlayers; i++){
			if(lobbyPlayerArray[i] == null){
				break;
			}
		}
		if(i >= maxPlayers) {
			conn.Disconnect();
            conn.Dispose();

            //tell the client
        }
        //		slotArray[i] = conn.playerControllers[0].gameObject;
        //		conn.playerControllers[0].gameObject.GetComponent<Player_ID>().SetSlot(i);
	}

    //Called on the server when a client disconnects.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect " + conn.connectionId);
        base.OnServerDisconnect(conn);

        switch (currentMode)
        {
            case NetworkMode.Lobby:
                Debug.Log("OnServerDisconnect Lobby ");

                bool hasError = true;
                for (int i = 0; i < maxPlayers; i++)
                {
                    if (lobbyPlayerArray[i] != null)
                    {
                        LobbyPlayer lp = (LobbyPlayer)lobbyPlayerArray[i];
                        int lpconnid = lp.connectionToClient.connectionId;
                        if(lpconnid == conn.connectionId)
                        {
                            hasError = false;
                            Debug.Log(i + " remove " + lpconnid);
                            lobbyPlayerArray[i] = null;
                        }
                    }
                }
                if (hasError) Debug.LogError("Error to remove the lobby when client disconnect.");
                break;
            case NetworkMode.Game:
                Debug.Log("OnServerDisconnect Game ");
                for (int i = 0; i < maxPlayers; i++)
                {
                    if (gameplayerControllers[i] != null)
                    {
                        // Game player to Disconnected Player
                        PlayerController pc = (PlayerController)gameplayerControllers[i];
                        int pcconnid = pc.connectionToClient.connectionId;
                        if (pcconnid == conn.connectionId)
                        {
                            Debug.Log(i + " set disconnected player " + pcconnid);
                            gameplayerControllers[i] = null;
                            GameObject disconPlayer = (GameObject)Instantiate(DisconnectedPlayerPrefab, Vector3.zero, Quaternion.identity);
                            disconnectedPlayerControllers[i] = disconPlayer.GetComponent<DisconnectedPlayerController>();
                            disconPlayer.GetComponent<DisconnectedPlayerController>().slot = i;
                            disconPlayer.GetComponent<DisconnectedPlayerController>().connId = conn.connectionId;
                            NetworkServer.Destroy(pc.gameObject);
                        }
                    }
                }
                break;

        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("OnServerAddPlayer " + conn.connectionId);

        switch (currentMode)
        {
            case NetworkMode.Lobby:
                Debug.Log("OnServerAddPlayer Lobby ");
                int i = 0;
                for (; i < maxPlayers; i++)
                    if (lobbyPlayerArray[i] == null) break;
                if (i == maxPlayers) return;
                var player = (GameObject)GameObject.Instantiate(LobbyPlayerPrefab, Vector3.zero, Quaternion.identity);
                player.GetComponent<LobbyPlayer>().slot = i;
                player.GetComponent<LobbyPlayer>().currentLobby = currentLobby;
                Debug.Log("OnServerAddPlayer Lobby " + player.GetComponent<LobbyPlayer>().currentLobby);

                lobbyPlayerArray[i] = player.GetComponent<LobbyPlayer>();

                //player.GetComponent<LobbyPlayer>().ToggleVisibility(false);
                NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

                break;
            case NetworkMode.Game:
                Debug.Log("OnServerAddPlayer Game ");
                bool isExistingPlayer = false;
                for (int k = 0; k < maxPlayers; k++)
                {
                    if (disconnectedPlayerControllers[k] != null)
                    {
                        DisconnectedPlayerController dpc = (DisconnectedPlayerController)disconnectedPlayerControllers[k];
                        int dpcconnid = dpc.connId;
                        // Disconnected player to Game Player
                        if (dpcconnid == conn.connectionId)
                        {
                            isExistingPlayer = true;
                            Debug.Log(k + " existing " + dpcconnid);
                            disconnectedPlayerControllers[k] = null;

                            GameObject gamePlayer = (GameObject)Instantiate(StrikerPrefab, Vector3.zero, Quaternion.identity);
                            gameplayerControllers[k] = gamePlayer.GetComponent<PlayerController>();
                            gamePlayer.GetComponent<PlayerController>().slot = k;
                            NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, 0);

                            Destroy(dpc.gameObject);
                        }
                    }
                }
                if (!isExistingPlayer)
                {
                    conn.Disconnect();
                    conn.Dispose();
                }
                break;
        }

    }
    #endregion

    #region ClientOverride
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnClientDisconnect " + conn.connectionId);

        base.OnClientDisconnect(conn);
        //return back to lobby
        ChangeTo(connectPanel);
        DisableGameUI();
        currentMode = NetworkMode.Lobby;
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnClientError " + conn.connectionId);

        ChangeTo(connectPanel);
        infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        DisableGameUI();
        currentMode = NetworkMode.Lobby;
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect " + conn.connectionId);

        base.OnClientConnect(conn);

        infoPanel.gameObject.SetActive(false);

        if (!NetworkServer.active)
        {//only to do on pure client (not self hosting client)
            ChangeTo(levelPanel);
            backDelegate = StopClientClbk;
            //SetServerInfo("Client", networkAddress);
        }
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("OnClientSceneChanged " + conn.connectionId);

        DisableLobbyUI();
        //ClientScene.Ready(connetion);
    }
    #endregion

    #region HostOverride
    public override void OnStartHost()
    {
        Debug.Log("OnStartHost");

        base.OnStartHost();

        ChangeTo(levelPanel);
        backDelegate = StopHostClbk;
        //SetServerInfo("Hosting", networkAddress);
    }
    #endregion

    private void ResetLobbyPlayerArray()
    {
        lobbyPlayerArray.Clear();
        for(int i = 0; i < maxPlayers; i++)
        {
            lobbyPlayerArray.Add(null);
        }
    }

    private void DisableGameUI()
    {
        GameObject[] gameUIs = GameObject.FindGameObjectsWithTag("GameUI");
        foreach (GameObject go in gameUIs)
        {
            Destroy(go);
        }
    }

    private void ChangeVisibilityOfLobbyPlayer(bool visible)
    {
        Debug.Log("ChangeVisibilityOfLobbyPlayer dsf");
        GameObject[] lps = GameObject.FindGameObjectsWithTag("LobbyPlayerUI");
        foreach(GameObject lp in lps)
        {
            if(lp != null)
            {
                lp.GetComponent<LobbyPlayer>().ToggleVisibility(visible);
            }
        }
    }

    private void ResetAfterStopServer()
    {
        Debug.Log("ResetAfterStopServer");
        currentMode = NetworkMode.Lobby;
        currentLobby = LobbyMode.Level;

        for (int i = 0; i < maxPlayers; i++)
        {
            lobbyPlayerArray[i] = null;
            gameplayerControllers[i] = null;
            disconnectedPlayerControllers[i] = null;
            levels[i] = LevelEnum.Unselected;
        }
        StopCoroutine("CheckLevelSelect");
        levelPanel.gameObject.GetComponent<LobbyLevelPanel>().ResetButtons();
    }
}
