#define debuglog

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityStandardAssets.Network;
using System.Collections.Generic;

public enum NetworkMode { Level, Lobby, Game };
//public enum LobbyMode { Level, Role };

public class NetworkManagerCustom : NetworkManager {
    public GameObject LobbyPlayerPrefab;
    public GameObject StrikerPrefab;
    public GameObject EngineerPrefab;
    public GameObject DisconnectedPlayerPrefab;

    public ArrayList gameplayerControllers { get; set; }
    public ArrayList lobbyPlayerArray { get; set; }
    private ArrayList disconnectedPlayerControllers;
    private HashSet<string> userNameSet;
    private NetworkMode currentMode;
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
    private bool hasCreatePlayer = false;
    void Start()
    {
        Debug.Log("Start");

#if UNITY_IOS
        serverConnect.SetActive(false);
#endif

        GameObject es = GameObject.Find("EventSystem");
        GameObject.DontDestroyOnLoad(es);

        SingletonNM = this;
        currentMode = NetworkMode.Level;
        //currentPanel = connectPanel;
        lobbyPlayerArray = new ArrayList(maxPlayers);
        gameplayerControllers = new ArrayList(maxPlayers);
        disconnectedPlayerControllers = new ArrayList(maxPlayers);
        userNameSet = new HashSet<string>();
        levels = new ArrayList(maxPlayers);
        for (int i = 0; i < maxPlayers; i++)
        {
            lobbyPlayerArray.Add(null);
            gameplayerControllers.Add(null);
            disconnectedPlayerControllers.Add(null);
            levels.Add(LevelEnum.Unselected);
        }

        lobbySystemStartSetting();

        //StartCoroutine(startLatency());
    }

    IEnumerator startLatency()
    {
        Debug.Log("startLatency");
        yield return new WaitForSeconds(15.0f);
        useSimulator = true;
    }

#region Lobby
    private void lobbySystemStartSetting()
    {
        //SetServerInfo("Offline", "None");
        currentPanel = connectPanel;
    }

    private void ChangeTo(RectTransform newPanel)
    {
        Debug.Log("ChangeTo " + newPanel.gameObject.name);
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
        //TODO: 
        //ChangeVisibilityOfLobbyPlayerOnLocal(false);
        ChangeTo(levelPanel);
    }

    public void ChangeToConnectPanel()
    {
        Debug.Log("ChangeToConnectPanel");

        ChangeTo(connectPanel);
    }
#endregion

    public void ChangeToSettingPanel()
    {
        Debug.Log("ChangeToSettingPanel");

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
            string debuglog = "CheckLevelSelect ";
            //TODO: bug
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
                    debuglog += curr + " ";
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
                else
                {
                    debuglog += " null  ";
                }
            }
            debuglog += " allSame " + isAllSame + " allselected " + isAllSelected;
            //Debug.Log(debuglog);

            if (userCount < minPlayers)
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
        Debug.Log("ChangeToLobbyPanel");

        StopCoroutine("CheckLevelSelect");
        //backDelegate = OnLobbyBackToLevelClbk;
        ChangeTo(lobbyPanel);
        currentMode = NetworkMode.Lobby;

        ChangeVisibilityOfLobbyPlayerEverywhere(true);
        //ask client to change panel
        ServerMessage sm = new ServerMessage();
        sm.currentMode = currentMode;
        NetworkServer.SendToAll(ServerMessage.MsgType, sm);
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
                    newPlayer.GetComponent<EngineerController>().username = lp.userName;
                    newPlayer.GetComponent<PlayerController>().cam.cullingMask = (1 << (i + 8)) | 1;
                    NetworkServer.Destroy(lp.gameObject);

                    ////Destroy(lp.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, newPlayer, id);
                   // Debug.Log("ReplacePlayerForConnection " + newPlayer.GetComponent<EngineerController>().teammatesInfo);
                }
                else {
                    GameObject newPlayer = (GameObject)Instantiate(StrikerPrefab, Vector3.zero, Quaternion.identity);
                    newPlayer.GetComponent<PlayerController>().slot = lp.slot;
                    gameplayerControllers[i] = newPlayer.GetComponent<PlayerController>();
                    //NetworkServer.Spawn(newPlayer);
                    short id = lp.GetComponent<LobbyPlayer>().playerControllerId;
                    //Debug.Log("playercontrollerid: " + id);
                    newPlayer.GetComponent<PlayerController>().role = lp.ownRole;
                    newPlayer.GetComponent<PlayerController>().username = lp.userName;

                    if (lp.ownRole == PlayerRole.Striker)
                        newPlayer.GetComponent<PlayerController>().cam.cullingMask = 1 << (8 + i);
                    
                    newPlayer.GetComponent<PlayerController>().setInGame();
                    NetworkServer.Destroy(lp.gameObject);

                    //Destroy(lp.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, newPlayer, id);
                    //Debug.Log("ReplacePlayerForConnection " + success);
                }
            }
        }

        SetupEngineerTeammateInfo();
        
        currentMode = NetworkMode.Game;
        ResetLobbyPlayerArray();
        ServerChangeScene(selectedLevel);
    }

    private void SetupEngineerTeammateInfo() {
        Debug.Log("Start to assign teammate");
        for (int i = 0; i < maxPlayers; i++)
        {
            PlayerController ipc = (PlayerController)gameplayerControllers[i];
            if (ipc != null && ipc.role == PlayerRole.Engineer)
            {
                EngineerController ec = (EngineerController)ipc;
                for (int j = 0; j < maxPlayers; j++)
                {
                    PlayerController jpc = (PlayerController)gameplayerControllers[j];
                    if(jpc != null)
                        ec.initializeTeammate(jpc.slot, jpc.role, jpc.username);
                }
            }
        }
    }

    private void SetUpEngineerTeammateInfo(EngineerController self) {
        for (int j = 0; j < maxPlayers; j++)
        {
            PlayerController jpc = (PlayerController)gameplayerControllers[j];
            if(jpc != null)
                self.initializeTeammate(jpc.slot, jpc.role, jpc.username);
        }
    }

    private void SetupGameScene()
    {
        Debug.Log("SetupGameScene");

        DisableLobbyUI();
    }

    public void DisableLobbyUI()
    {
        Debug.Log("DisableLobbyUI");

        topPanel.gameObject.SetActive(false);
        connectPanel.gameObject.SetActive(false);
        lobbyPanel.gameObject.SetActive(false);
        infoPanel.gameObject.SetActive(false);
    }

    public void SetLevelWithSlot(LevelEnum le, int newSlot)
    {
        Debug.Log("SetLevelWithSlot");

        if (newSlot >=0 && newSlot < maxPlayers)
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
        base.OnStartServer();
        isServer = true;
        currentMode = NetworkMode.Level;
        StartCoroutine("CheckLevelSelect");
        // change first, if it is a host, it will change back to level.
        ChangeTo(lobbyPanel);
    }

    //This hook is called when a server is stopped - including when a host is stopped.
    public override void OnStopServer()
    {
        Debug.Log("OnStopServer");
        base.OnStopServer();
        currentMode = NetworkMode.Level;
        StopCoroutine("CheckLobbyReady");
        ResetLobbyPlayerArray();
        ResetAfterStopServer();
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
        base.OnServerConnect(conn);
        //ask client to change panel
        ServerMessage sm = new ServerMessage();
        sm.currentMode = currentMode;
        sm.isCreatePlayer = true;
        NetworkServer.SendToClient(conn.connectionId, ServerMessage.MsgType, sm);
    }

    //Called on the server when a client disconnects.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect " + conn.connectionId);
        base.OnServerDisconnect(conn);
        
        switch (currentMode)
        {
            //just removing the corresponding object in lobbyPlayerArray
            case NetworkMode.Level:
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
                            userNameSet.Remove(lp.userName);
                        }
                    }
                }
                if (hasError) Debug.LogError("Error to remove the lobby when client disconnect.");
                break;
            // create a disconnect player to store the data, preparing for reconnect.
            //TODO: why not store the object itself
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
                            //conn.isReady = false;
                            gameplayerControllers[i] = null;
                            GameObject disconPlayer = (GameObject)Instantiate(DisconnectedPlayerPrefab, Vector3.zero, Quaternion.identity);
                            disconnectedPlayerControllers[i] = disconPlayer.GetComponent<DisconnectedPlayerController>();
                            disconPlayer.GetComponent<DisconnectedPlayerController>().slot = i;
                            disconPlayer.GetComponent<DisconnectedPlayerController>().connId = conn.connectionId;
                            disconPlayer.GetComponent<DisconnectedPlayerController>().currentRole = pc.role;
                            disconPlayer.GetComponent<DisconnectedPlayerController>().username = pc.username;
                            disconPlayer.GetComponent<DisconnectedPlayerController>().health = pc.gameObject.GetComponent<PlayerInfo>().getHealth();

                            NetworkServer.Destroy(pc.gameObject);
                            conn.isReady = false;
                            Debug.Log("disconnect " + conn.isReady);
                        }
                    }
                }
                break;

        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        Debug.Log("OnServerAddPlayer " + conn.connectionId);
        //cannot add base.OnServerAddPlayer(conn, playerControllerId);
        string playerName = "";
        if (extraMessageReader != null)
        {
            var s = extraMessageReader.ReadMessage<PlayerMessage>();
            playerName = s.userName;
            Debug.Log("my name is " + s.userName);
        }
        switch (currentMode)
        {
            case NetworkMode.Level: 
            case NetworkMode.Lobby:
                Debug.Log("OnServerAddPlayer Lobby ");
                //find out the name does not exist
                if(!CheckUserNameValid(playerName)){
                    ErrorMessage em = new ErrorMessage();
                    em.errorMessage = "This Account Has Already Connected to Server.";
                    NetworkServer.SendToClient(conn.connectionId, ErrorMessage.MsgType, em);
                    return;
                }
                //find a empty slot
                int i = 0;
                for (; i < maxPlayers; i++)
                    if (lobbyPlayerArray[i] == null) break;
                if (i == maxPlayers) return;
                //create lobby object
                var player = (GameObject)GameObject.Instantiate(LobbyPlayerPrefab, Vector3.zero, Quaternion.identity);
                player.GetComponent<LobbyPlayer>().slot = i;
                player.GetComponent<LobbyPlayer>().currentMode = currentMode;

                Debug.Log("OnServerAddPlayer Lobby ");

                lobbyPlayerArray[i] = player.GetComponent<LobbyPlayer>();

                //player.GetComponent<LobbyPlayer>().ToggleVisibility(false);
                NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
                if(currentMode == NetworkMode.Lobby)
                { // make lobby visible amoung all clients and server
                    ChangeVisibilityOfLobbyPlayerEverywhere(true);
                }
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
                        if (dpc.username == playerName)
                        {
                            //TODO: Disconnect to Player
                            isExistingPlayer = true;
                            Debug.Log(k + " existing " + dpcconnid);
                            disconnectedPlayerControllers[k] = null;

                            GameObject selectedprefab = StrikerPrefab;
                            switch (dpc.currentRole)
                            {
                                case PlayerRole.Engineer:
                                    selectedprefab = EngineerPrefab;
                                    Debug.Log("Reconnect EngineerPrefab");
                                    break;
                            }

                            GameObject gamePlayer = (GameObject)Instantiate(selectedprefab, Vector3.zero, Quaternion.identity);
                            gameplayerControllers[k] = gamePlayer.GetComponent<PlayerController>();
                            gamePlayer.GetComponent<PlayerController>().slot = k;
                            gamePlayer.GetComponent<PlayerController>().username = dpc.username;
                            gamePlayer.GetComponent<PlayerController>().role = dpc.currentRole;
                            gamePlayer.GetComponent<PlayerController>().setInGame();

                            if(gamePlayer.GetComponent<PlayerController>().role == PlayerRole.Engineer)
                                SetUpEngineerTeammateInfo(gamePlayer.GetComponent<EngineerController>());
                            

                            NetworkServer.AddPlayerForConnection(conn, gamePlayer, playerControllerId);
                            Destroy(dpc.gameObject);
                        }
                    }
                }
                if (!isExistingPlayer)
                {
                    Debug.Log("No Existing User on the Server");
                    ErrorMessage em = new ErrorMessage();
                    em.errorMessage = "No Existing User on the Server";
                    NetworkServer.SendToClient(conn.connectionId, ErrorMessage.MsgType, em);
                }
                break;
        }

    }

    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log("OnServerReady " + conn.connectionId);
        base.OnServerReady(conn);

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
        currentMode = NetworkMode.Level;
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnClientError " + conn.connectionId);

        ChangeTo(connectPanel);
        infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        DisableGameUI();
        currentMode = NetworkMode.Level;
    }

    public override void OnClientNotReady(NetworkConnection conn)
    {
        Debug.Log("OnClientNotReady " + conn.connectionId);

    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect " + conn.connectionId);
        Debug.Log("SetReady " + conn.isReady);
        Debug.Log("SetReady2 " + ClientScene.ready);
        //base.OnClientConnect(conn);
        client.RegisterHandler(ServerMessage.MsgType, OnServerMessageReceived);
        client.RegisterHandler(ErrorMessage.MsgType, OnErrorShow);


        infoPanel.gameObject.SetActive(false);
        //ChangeTo(levelPanel);
        if (!NetworkServer.active)
        {//only to do on pure client (not self hosting client)
            Debug.Log("pure client");
            //ChangeTo(levelPanel);
            backDelegate = StopClientClbk;
            //SetServerInfo("Client", networkAddress);
        }
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("OnClientSceneChanged " + conn.connectionId);
        Debug.Log("SetReady " + conn.isReady);
        Debug.Log("SetReady2 " + ClientScene.ready);

        DisableLobbyUI();
        //ClientScene.Ready(conn);

        if (!ClientScene.ready)
        {
            Debug.Log("SetReady");
            base.OnClientSceneChanged(conn);
            if (!hasCreatePlayer) {
                PlayerMessage pm = new PlayerMessage();
                pm.userName = LoginController.userName;
                ClientScene.AddPlayer(client.connection, 0, pm);
                hasCreatePlayer = true;
            }
        }
        Debug.Log("SetReady " + conn.isReady);
        Debug.Log("SetReady2 " + ClientScene.ready);
        //ClientScene.Ready(connetion);
    }
#endregion

#region HostOverride
    public override void OnStartHost()
    {
        Debug.Log("OnStartHost");

        base.OnStartHost();

        //ChangeTo(levelPanel);
        backDelegate = StopHostClbk;
        //SetServerInfo("Hosting", networkAddress);
    }

    public override void OnStopHost()
    {
        Debug.Log("OnStartHost");

        base.OnStopHost();
        ResetAfterStopServer();

    }
    #endregion

    #region Message
    //change panel on client
    public void OnServerMessageReceived(NetworkMessage msg)
    {
        ServerMessage mx = msg.ReadMessage<ServerMessage>();
        Debug.Log(string.Format("SERVER: {0}", mx));

        switch (mx.currentMode)
        {
            case NetworkMode.Level:
                Debug.Log("OnServerMessageReceived Change levelPanel");
                ChangeTo(levelPanel);
                if (mx.isCreatePlayer)
                {
                    PlayerMessage pm = new PlayerMessage();
                    pm.userName = LoginController.userName;
                    ClientScene.AddPlayer(client.connection, 0, pm);
                    hasCreatePlayer = true;
                }
                break;
            case NetworkMode.Lobby:
                Debug.Log("OnServerMessageReceived Change lobbyPanel");
                ChangeTo(lobbyPanel);
                if (mx.isCreatePlayer)
                {
                    PlayerMessage pm = new PlayerMessage();
                    pm.userName = LoginController.userName;
                    ClientScene.AddPlayer(client.connection, 0, pm);
                    hasCreatePlayer = true;
                }
                break;
            case NetworkMode.Game:
                Debug.Log("OnServerMessageReceived Change Game");
                //Application.LoadLevel("Level1");
                break;
        }
    }

    public void OnErrorShow(NetworkMessage msg)
    {
        Debug.Log("OnErrorShow");

        ErrorMessage em = msg.ReadMessage<ErrorMessage>();
        Debug.Log(em);
        infoPanel.Display(em.errorMessage, "Close", StopClientClbk);
    }
#endregion
    private void ResetLobbyPlayerArray()
    {
        Debug.Log("ResetLobbyPlayerArray");

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

    private void ChangeVisibilityOfLobbyPlayerEverywhere(bool visible)
    {
        Debug.Log("ChangeVisibilityOfLobbyPlayerEverywhere");
        GameObject[] lps = GameObject.FindGameObjectsWithTag("LobbyPlayerUI");
        foreach(GameObject lp in lps)
        {
            if(lp != null)
            {
                lp.GetComponent<LobbyPlayer>().ServerToggleVisibility(visible);
            }
        }
    }

    private void ResetAfterStopServer()
    {
        Debug.Log("ResetAfterStopServer");
        currentMode = NetworkMode.Level;
        userNameSet.Clear();
        for (int i = 0; i < maxPlayers; i++)
        {
            lobbyPlayerArray[i] = null;
            gameplayerControllers[i] = null;
            disconnectedPlayerControllers[i] = null;
            levels[i] = LevelEnum.Unselected;
        }
        StopCoroutine("CheckLevelSelect");
        StopCoroutine("CheckLobbyReady");
        levelPanel.gameObject.GetComponent<LobbyLevelPanel>().ResetButtons();
    }

    public void ShowWarning(string displayMessage, string buttonMessage)
    {
        infoPanel.Display(displayMessage, buttonMessage, null);
    }

    public bool CheckUserNameValid(string newName) {
        if (userNameSet.Contains(newName))
        {
            return false;
        }
        else
        {
            userNameSet.Add(newName);
            return true;
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application Quit");

        Debug.Log("StopClient");
        StopClient();
        if (isServer)
        {
            Debug.Log("StopServer");
            StopServer();
        }

    }


    #region Feiran's Function - Never Touch!

    public void AttackPlayer(int index, float damage) {

        if (gameplayerControllers[index] != null)
            ((PlayerController)gameplayerControllers[index]).RpcDamage(damage);

    }

    #endregion
}
