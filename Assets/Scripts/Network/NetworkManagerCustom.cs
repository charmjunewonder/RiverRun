﻿#define debuglog

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityStandardAssets.Network;
using System.Collections.Generic;

public enum NetworkMode { Level, Lobby, Game, Complete };
//public enum LobbyMode { Level, Role };

public class NetworkManagerCustom : NetworkManager {
    public GameObject LobbyPlayerPrefab;
    public GameObject StrikerPrefab;
    public GameObject EngineerPrefab;
    public GameObject DisconnectedPlayerPrefab;
    public MissionPanel missionPanel;
    public ServerMissionCompletePanel serverMissionPanel;
    public ServerGamePanel sGamePanel;
    public ProgressBarController pbController;
    public GameObject playerData;

    public ArrayList gameplayerControllers { get; set; }
    public ArrayList lobbyPlayerArray { get; set; }
    public ArrayList disconnectedPlayerControllers{ get; set; }
    private HashSet<string> userNameSet;
    private NetworkMode currentMode;
    public GameObject serverConnect;
    static public NetworkManagerCustom SingletonNM { get; set;}
    
    #region Lobby Variable

    public int maxPlayers = 4;
    public int minPlayers = 1;

    public LobbyConnectPanel topPanel;

    public RectTransform gameConnectPanel;
    public RectTransform connectPanel;
    public RectTransform levelPanel;
    public RectTransform lobbyPanel;
    public RectTransform settingPanel;

    public LobbyInfoPanel infoPanel;

    protected RectTransform currentPanel;
    protected RectTransform previousPanel;

    #endregion

    private string selectedLevel = "Level1";
    public LevelEnum selectedDifficulty;
    private ArrayList levels;
    private bool isServer = false;
    private bool hasCreatePlayer = false;

    private float cizitenshipHealth = 10;
    private int citizenshipZeroTime = 0;

    public bool isPause = false;
    private PlayerSpaceshipController spaceshipCon;
    void Start()
    {

#if UNITY_IOS
        serverConnect.SetActive(false);
        gameConnectPanel.gameObject.SetActive(false);
        connectPanel.gameObject.SetActive(true);
#endif
        //logLevel = LogFilter.FilterLevel.Debug;
        GameObject es = GameObject.Find("EventSystem");
        GameObject.DontDestroyOnLoad(es);
        isPause = false;
        SingletonNM = this;
        currentMode = NetworkMode.Level;
        selectedDifficulty = selectedDifficulty = LevelEnum.Unselected;
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
        citizenshipZeroTime = 0;
        //StartCoroutine(startLatency());
        //infoPanel.DisplayDisconnectError("Client error : ", StopClientClbk, ReconnectClinetClbk);
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

        if (currentPanel == levelPanel)
        {
            levelPanel.gameObject.GetComponent<LobbyLevelPanel>().ResetButtons();
            //SetServerInfo("Offline", "None");
        }
    }

    public void DisplayIsConnecting()
    {
        var _this = this;
        infoPanel.DisplayWarning("Connecting to " + networkAddress, () => { _this.backDelegate(); });
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

    public void ReconnectClinetClbk()
    {
        StopClient();
        DisableGameUI();
        //Application.LoadLevel("Lobby");
        hasCreatePlayer = false;
        connectPanel.gameObject.GetComponent<LobbyConnectPanel>().OnClickJoin();
    }
#endregion

    public void ChangeToSettingPanel()
    {
        Debug.Log("ChangeToSettingPanel");

        ChangeTo(settingPanel);
        //backDelegate = SimpleBackClbk;
    }

    public void ChangeToServerSettingPanel()
    {
        Debug.Log("ChangeToSettingPanel");
        currentPanel = gameConnectPanel;
        ChangeTo(settingPanel);
        //backDelegate = SimpleBackClbk;
    }

    IEnumerator CheckLobbyReady()
    {
        Debug.Log("CheckLobbyReady");
        while (true)
        {
            //Debug.Log("CheckLobbyReady inside");

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
            //Debug.Log("CheckLevelSelect");
            //string debuglog = "CheckLevelSelect ";
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
                    //debuglog += curr + " ";
                    if (isFirst) {
                        isAllSame = true;
                        isAllSelected = true;
                        firstLevel = curr;
                        //Debug.Log("Level Select Same  " + firstLevel.ToString());
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
                    //debuglog += " null  ";
                }
            }
            //debuglog += " allSame " + isAllSame + " allselected " + isAllSelected;
            //Debug.Log(debuglog);

            if (userCount < minPlayers)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }
            if (isAllSame)
            {
                Debug.Log("Level Select Same");
                selectedDifficulty = firstLevel;
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
        ResetLevelSelectUI();
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
                lp.ResetUI();
                NetworkConnection conn = lp.connectionToClient;
                if (lp.ownRole == PlayerRole.Engineer){

                    GameObject newPlayer = (GameObject)Instantiate(EngineerPrefab, Vector3.zero, Quaternion.identity);

                    newPlayer.GetComponent<EngineerController>().slot = lp.slot;
                    gameplayerControllers[i] = newPlayer.GetComponent<EngineerController>();
                    ////NetworkServer.Spawn(newPlayer);
                    short id = lp.GetComponent<LobbyPlayer>().playerControllerId;
                    ////Debug.Log("playercontrollerid: " + id);
                    newPlayer.GetComponent<EngineerController>().role = lp.ownRole;
                    newPlayer.GetComponent<EngineerController>().username = lp.userName;
                    newPlayer.GetComponent<PlayerController>().cam.cullingMask = (1 << (i + 8)) | 1;
                    newPlayer.GetComponent<PlayerController>().playerParameter = playerData.GetComponent<PlayerParameter>();
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
                    newPlayer.GetComponent<PlayerController>().playerParameter = playerData.GetComponent<PlayerParameter>();

                    if (lp.ownRole == PlayerRole.Striker)
                        newPlayer.GetComponent<PlayerController>().cam.cullingMask = (1 << (8 + i)) | 1;
                    
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
        SetServerGamePanel();
        StartCoroutine("HealthPenalty");

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

    private void SetUpEngineerTeammateInfo(EngineerController ec) {
        for (int j = 0; j < maxPlayers; j++)
        {
            PlayerController jpc = (PlayerController)gameplayerControllers[j];
            if (jpc != null) {
                ec.initializeTeammate(jpc.slot, jpc.role, jpc.username);
            }
                
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
            //Debug.Log("Set Level " + le.ToString() + " " + newSlot);
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
        gameConnectPanel.gameObject.SetActive(false);

        ChangeTo(lobbyPanel);
    }

    //This hook is called when a server is stopped - including when a host is stopped.
    public override void OnStopServer()
    {
        Debug.Log("OnStopServer");
        base.OnStopServer();
        currentMode = NetworkMode.Level;
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
            Debug.Log("**********************OnServerConnect Disconnect the Client**********************");
            Debug.Log("**********************OnServerConnect Disconnect the Client**********************");
            Debug.Log("**********************OnServerConnect Disconnect the Client**********************");

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
                            userNameSet.Remove(lp.userName.ToLower());
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
                            DisconnectedPlayerController dpc = disconPlayer.GetComponent<DisconnectedPlayerController>();
                            dpc.slot = i;
                            dpc.connId = conn.connectionId;
                            dpc.currentRole = pc.role;
                            dpc.username = pc.username;
                            dpc.health = pc.gameObject.GetComponent<PlayerInfo>().getHealth();
                            dpc.crystals = pc.disconnectCrystal;
                            dpc.rank = pc.rank;
                            dpc.exp = pc.exp;
                            dpc.score = pc.score;
                            dpc.skill1Counter = pc.skill1Counter;
                            dpc.skill2Counter = pc.skill2Counter;
                            dpc.supportCounter = pc.supportCounter;
                            dpc.isPause = pc.isPause;
                            if (UltiController.checkUltiEnchanting()) {
                                if (UltiController.getUltiPlayerNumber() == i){
                                    UltiController.setUltiEnchanting(false);
                                }

                                for (int k = 0; k < maxPlayers; k++) {
                                    if (gameplayerControllers[k] != null && k != i) {
                                        ((PlayerController)gameplayerControllers[k]).UnlockUlti();
                                    }
                                }
                            }

                            NetworkServer.Destroy(pc.gameObject);
                            conn.isReady = false;
                            Debug.Log("disconnect " + conn.isReady);
                        }
                    }
                }
                break;
            case NetworkMode.Complete:
                Debug.Log("OnServerDisconnect Complete ");
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
                player.GetComponent<LobbyPlayer>().ownRole = PlayerRole.Unselected;

                //player.GetComponent<LobbyPlayer>().currentMode = currentMode;

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
                            PlayerController pc = gamePlayer.GetComponent<PlayerController>();
                            pc.slot = k;
                            pc.username = dpc.username;
                            pc.role = dpc.currentRole;
                            pc.rank = dpc.rank;
                            pc.exp = dpc.exp;
                            pc.score = dpc.score;
                            pc.skill1Counter = dpc.skill1Counter;
                            pc.skill2Counter = dpc.skill2Counter;
                            pc.supportCounter = dpc.supportCounter;
                            pc.setInGame();
                            pc.disconnectCrystal = dpc.crystals;
                            pc.InitializeDisconnectCrystals(dpc.crystals);
                            pc.isPause = dpc.isPause;

                            if (dpc.currentRole == PlayerRole.Engineer)
                            {
                                SetUpEngineerTeammateInfo(gamePlayer.GetComponent<EngineerController>());
                                pc.GetComponent<EngineerController>().initializeData();
                            }
                            else
                            {
                                pc.GetComponent<PlayerController>().initializeData();
                            }

                            Debug.Log("Network Manager InitializeData");

                            gamePlayer.GetComponent<PlayerInfo>().setHealth(dpc.health);

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
            case NetworkMode.Complete:
                ErrorMessage eem = new ErrorMessage();
                eem.errorMessage = "Game Ended!";
                NetworkServer.SendToClient(conn.connectionId, ErrorMessage.MsgType, eem);
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
        missionPanel.gameObject.SetActive(false);
        currentMode = NetworkMode.Level;
        Application.LoadLevel("Empty");

    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnClientError " + conn.connectionId);

        ChangeTo(connectPanel);
        infoPanel.DisplayDisconnectError("Client error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()),
            StopClientClbk, ReconnectClinetClbk);
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
        //Debug.Log(em);
        infoPanel.DisplayWarning(em.errorMessage, StopClientClbk);
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
        Debug.Log("DisableGameUI");

        GameObject[] gameUIs = GameObject.FindGameObjectsWithTag("GameUI");
        foreach (GameObject go in gameUIs)
        {
            Destroy(go);
        }
    }

    private void DestroyLobbyPlayerUI()
    {
        Debug.Log("DisableLobbyUI");

        GameObject[] lobbyUIs = GameObject.FindGameObjectsWithTag("LobbyPlayerUI");
        foreach (GameObject lo in lobbyUIs)
        {
            Destroy(lo);
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
        hasCreatePlayer = false;
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
        StopCoroutine(HealthPenalty());

        sGamePanel.Reset();
        sGamePanel.gameObject.SetActive(false);
        serverMissionPanel.gameObject.SetActive(false);
        DestroyLobbyPlayerUI();
        levelPanel.gameObject.GetComponent<LobbyLevelPanel>().ResetButtons();
    }

    public void ShowWarning(string displayMessage)
    {
        infoPanel.DisplayWarning(displayMessage, null);
    }

    public bool CheckUserNameValid(string newName) {
        if (userNameSet.Contains(newName.ToLower()))
        {
            return false;
        }
        else
        {
            userNameSet.Add(newName.ToLower());
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

    void ResetLevelSelectUI()
    {
        levelPanel.GetComponent<LobbyLevelPanel>().ResetButtons();
    }

    private void SetServerGamePanel()
    {
        sGamePanel.gameObject.SetActive(true);
        for (int i = 0; i < maxPlayers; i++)
        {
            PlayerController lp = (PlayerController)gameplayerControllers[i];
            if (lp != null)
            {
                Debug.Log("SetServerMissionCompletePanel");
                sGamePanel.playerInfos[i].gameObject.SetActive(true);
                sGamePanel.playerInfos[i].SetUserInfo(lp.username, lp.role, 1);
            }
            else
            {
                DisconnectedPlayerController dpc = (DisconnectedPlayerController)disconnectedPlayerControllers[i];
                if (dpc != null)
                {
                    sGamePanel.playerInfos[i].gameObject.SetActive(true);
                    sGamePanel.playerInfos[i].SetUserInfo(lp.username, lp.role, 1);
                }
            }
        }
    }

    public void OnRestartClicked()
    {
        StopServerClbk();
        ServerChangeScene("Empty");
        //StartCoroutine(waitAndStartServer());
    }

    IEnumerator waitAndStartServer()
    {
        yield return new WaitForSeconds(0.5f);
        StartServer();
    }

    public void PauseTheGame()
    {
        Debug.Log("PauseTheGame");
        PauseAllEnemy();
        for (int i = 0; i < maxPlayers; i++)
        {
            PlayerController lp = (PlayerController)gameplayerControllers[i];
            if (lp != null)
            {
                //lp.RpcPauseGame(true);
                lp.isPause = true;
            }
            else
            {
                DisconnectedPlayerController dpc = (DisconnectedPlayerController)disconnectedPlayerControllers[i];
                if (dpc != null)
                {
                    dpc.isPause = true;
                }
            }
        }
    }

    public void UnpauseTheGame()
    {
        Debug.Log("UnpauseTheGame");
        UnpauseAllEnemy();
        for (int i = 0; i < maxPlayers; i++)
        {
            PlayerController lp = (PlayerController)gameplayerControllers[i];
            if (lp != null)
            {
                //lp.RpcPauseGame(false);
                lp.isPause = false;
            }
            else
            {
                DisconnectedPlayerController dpc = (DisconnectedPlayerController)disconnectedPlayerControllers[i];
                if (dpc != null)
                {
                    dpc.isPause = false;
                }
            }
        }
    }

    public void PauseAllEnemy()
    {
        GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemySpawnManager>().Pause();
        GameObject.FindGameObjectWithTag("EnemySkillController").GetComponent<EnemyAttackFreezer>().Pause();
    }

    public void UnpauseAllEnemy()
    {
        GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemySpawnManager>().UnPause();
        GameObject.FindGameObjectWithTag("EnemySkillController").GetComponent<EnemyAttackFreezer>().UnPause();
    }

    #region Feiran's Functions

    public void AddProgress(float perc)
    {
        pbController.SetProgress(perc);
    }

    public void SetTotalEnemyWave(int num) {
        for (int k = 0; k < maxPlayers; k++)
        {
            if (gameplayerControllers[k] != null)
            {
                PlayerController pc = (PlayerController)gameplayerControllers[k];
                pc.RpcSetTotalWave(num);
            }
        }
    }

    public void SetCurrentEnemyWave(int num) {
        for (int k = 0; k < maxPlayers; k++)
        {
            if (gameplayerControllers[k] != null)
            {
                PlayerController pc = (PlayerController)gameplayerControllers[k];
                pc.RpcSetCurrentWave(num);
            }
        }
    }

    public void ClosePortal() {
        GameObject portal = GameObject.Find("newPortal-blue");
        for (int i = 1; i < portal.transform.childCount; i++)
        {
            portal.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int k = 0; k < maxPlayers; k++)
        {
            if (gameplayerControllers[k] != null)
            {
                PlayerController pc = (PlayerController)gameplayerControllers[k];
                pc.RpcClosePortal();
            }
        }
    }

    public void AttackCitizenship(float f) {
        cizitenshipHealth -= f;

        cizitenshipHealth = Mathf.Clamp(cizitenshipHealth, 0, 10);

        for (int k = 0; k < maxPlayers; k++) {
            if (gameplayerControllers[k] != null)
            {
                PlayerController pc = (PlayerController)gameplayerControllers[k];
                pc.RpcSetCitizenshipHealth(cizitenshipHealth);
            }
        }
        
        sGamePanel.SetCitizenShipHealth(cizitenshipHealth / 10);
    }

    public void AttackPlayer(int index, float damage) {

        if (gameplayerControllers[index] != null)
            ((PlayerController)gameplayerControllers[index]).Damage(damage);

    }


    IEnumerator HealthPenalty()
    {
        while (true)
        {
            float playTotalHP = 0;
            for (int k = 0; k < maxPlayers; k++)
            {
                if (gameplayerControllers[k] != null)
                {
                    PlayerController pc = (PlayerController)gameplayerControllers[k];
                    float hp = pc.GetComponent<PlayerInfo>().getHealth();
                    playTotalHP += (hp / pc.GetComponent<PlayerInfo>().max_health)*10;
                    if (hp == 0)
                    {
                        int pernalty = (int)(pc.score * ScoreParameter.Personal_Health_Penalty_Persent);
                        pc.score -= pernalty;
                    }
                }
                //else if (disconnectedPlayerControllers[k] != null)
                //{

                //}
            }
            //Debug.Log("playTotalHP "+playTotalHP);
            if (cizitenshipHealth <= 0)
            {
                citizenshipZeroTime++;
            }
            if (spaceshipCon != null)
            {

                spaceshipCon.SetCitizenShipDamage(cizitenshipHealth);
                spaceshipCon.SetPlayerSpaceshipDamage(playTotalHP);
            }
            else
            {
                GameObject goo = GameObject.Find("playerSpaceshipWithEffect(Clone)");
                if(goo != null)
                    spaceshipCon = goo.GetComponent<PlayerSpaceshipController>();
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void EndGame() {
        Debug.Log("EndGame");
        Invoke("GameEnded", 3.0f);
    }

    private void GameEnded() {
        currentMode = NetworkMode.Complete;
        sGamePanel.Reset();
        sGamePanel.gameObject.SetActive(false);
        int totalScore = 0;
        StopCoroutine("HealthPenalty");

        ArrayList names = new ArrayList();
        for (int k = 0; k < maxPlayers; k++)
        {
            if (gameplayerControllers[k] != null )
            {
                PlayerController gpc = (PlayerController)gameplayerControllers[k];
                //int score = ScoreParameter.CalcuateScore(gpc.skill1Counter, gpc.skill2Counter, gpc.supportCounter);
                int currentFullExp = ScoreParameter.CurrentFullExp(gpc.rank);
                int cexp = gpc.exp + gpc.score;
                int crank = gpc.rank;
                while (cexp > currentFullExp)
                {
                    cexp -= currentFullExp;
                    crank++;
                    currentFullExp = ScoreParameter.CurrentFullExp(crank);
                }
                gpc.rank = crank;
                gpc.exp = cexp;
                gpc.RpcMissionComplete(gpc.skill1Counter, gpc.skill2Counter, gpc.supportCounter, crank, cexp);
                totalScore += gpc.score;
                names.Add(gpc.username);
                DataServerUtil.Singleton.SendPersonalRecord(gpc.username, gpc.score, crank, gpc.role.ToString().ToLower(), cexp);
            }
        }
        totalScore += (int)((600 - EnemySpawnManager.currentTime) * 1.5f);
        citizenshipZeroTime = Mathf.Clamp(citizenshipZeroTime, 0, 40);
        int citizenPenalty = (int)(totalScore * citizenshipZeroTime * 0.01f);
        totalScore -= citizenPenalty;
        serverMissionPanel.gameObject.SetActive(true);
        string condistionString = "Perfect";
        if (cizitenshipHealth < 3) condistionString = "Low";
        else if (cizitenshipHealth < 6) condistionString = "Good";
        serverMissionPanel.SetServerMissionCompletePanel(totalScore, EnemySpawnManager.currentTime, condistionString);
        //send team record to data server
        DataServerUtil.Singleton.SendTeamRecord(names, totalScore);
        Debug.Log("MissinComplete Time: " + EnemySpawnManager.currentTime);

        ServerChangeScene("Empty");
    }

    public void MissionComplete(int score, int star, string username, PlayerRole pr, int rank, int experience, int fullEx, int skill1, int ulti, int support)
    {
        Debug.Log("MissinComplete");
        missionPanel.gameObject.SetActive(true);
        missionPanel.SetMissonComplete(score, star, username, pr, rank, experience, fullEx, skill1, ulti, support);
    }
    #endregion
}
