using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Network;

public enum NetworkMode { Lobby, Game };

public class NetworkManagerCustom : NetworkManager {
    public GameObject LobbyPlayerPrefab;
    public GameObject StrikerPrefab;
    public GameObject DisconnectedPlayerPrefab;

    public ArrayList gameplayerControllers { get; set; }
    public ArrayList lobbyPlayerArray { get; set; }
    public ArrayList disconnectedPlayerControllers { get; set; }
    public NetworkMode currentMode { get; set; }

    static public NetworkManagerCustom SingletonNM { get; set;}

    #region Lobby Variable

    public int maxPlayers = 4;
    public int minPlayers = 1;

    public LobbyTopPanel topPanel;

    public RectTransform mainMenuPanel;
    public RectTransform lobbyPanel;

    public LobbyInfoPanel infoPanel;

    protected RectTransform currentPanel;

    public Button backButton;

    public Text statusInfo;
    public Text hostInfo;

    #endregion

    private string selectedLevel = "Level1";

    void Start()
    {
        SingletonNM = this;
        currentMode = NetworkMode.Lobby;
        lobbyPlayerArray = new ArrayList(maxPlayers);
        gameplayerControllers = new ArrayList(maxPlayers);
        disconnectedPlayerControllers = new ArrayList(maxPlayers);
        for (int i = 0; i < maxPlayers; i++)
        {
            lobbyPlayerArray.Add(null);
            gameplayerControllers.Add(null);
            disconnectedPlayerControllers.Add(null);
        }

        lobbySystemStartSetting();


    }

    #region Lobby
    private void lobbySystemStartSetting()
    {
        backButton.gameObject.SetActive(false);
        SetServerInfo("Offline", "None");
        currentPanel = mainMenuPanel;
    }


    public void ChangeTo(RectTransform newPanel)
    {
        if (currentPanel != null)
        {
            currentPanel.gameObject.SetActive(false);
        }

        if (newPanel != null)
        {
            newPanel.gameObject.SetActive(true);
        }

        currentPanel = newPanel;

        if (currentPanel != mainMenuPanel)
        {
            backButton.gameObject.SetActive(true);
        }
        else
        {
            backButton.gameObject.SetActive(false);
            SetServerInfo("Offline", "None");
        }
    }

    public void DisplayIsConnecting()
    {
        var _this = this;
        infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
    }

    public void SetServerInfo(string status, string host)
    {
        statusInfo.text = status;
        hostInfo.text = host;
    }

    #region Disconnect Button
    public delegate void BackButtonDelegate();
    public BackButtonDelegate backDelegate;
    public void GoBackButton()
    {
        backDelegate();
    }

    public void SimpleBackClbk()
    {
        Debug.Log("SimpleBackClbk");
        ChangeTo(mainMenuPanel);
    }

    public void StopHostClbk()
    {
        Debug.Log("StopHostClbk");
        StopHost();
        ChangeTo(mainMenuPanel);
    }

    public void StopClientClbk()
    {
        Debug.Log("StopClientClbk");
        StopClient();
        ChangeTo(mainMenuPanel);
    }

    public void StopServerClbk()
    {
        Debug.Log("StopServerClbk");
        StopServer();
        ChangeTo(mainMenuPanel);
    }

    public void StopGameClbk()
    {
        Debug.Log("StopGameClbk");
        //SendReturnToLobby();
        ChangeTo(lobbyPanel);
    }
    #endregion

    IEnumerator CheckLobbyReady()
    {
        Debug.Log("CheckLobbyReady");
        while (true)
        {
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
                ChangeLobbyToGameScene();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void ChangeLobbyToGameScene()
    {
        StopCoroutine("CheckLobbyReady");
        DisableLobbyUI();
        //RpcSetupGameScene();
        for (int i = 0; i < maxPlayers; i++)
        {
            if (lobbyPlayerArray[i] != null)
            {
                LobbyPlayer lp = (LobbyPlayer)lobbyPlayerArray[i];
                NetworkConnection conn = lp.connectionToClient;
                GameObject newPlayer = (GameObject)Instantiate(StrikerPrefab, Vector3.zero, Quaternion.identity);
                newPlayer.GetComponent<PlayerController>().slot = lp.slot;
                gameplayerControllers[i] = newPlayer.GetComponent<PlayerController>();
                NetworkServer.Spawn(newPlayer);
                short id = lp.GetComponent<LobbyPlayer>().playerControllerId;
                Debug.Log("playercontrollerid: " + id);
                newPlayer.GetComponent<PlayerController>().role = lp.ownRole;
                NetworkServer.Destroy(lp.gameObject);
                
                //Destroy(lp.gameObject);

                bool success = NetworkServer.ReplacePlayerForConnection(conn, newPlayer, id);
                Debug.Log("ReplacePlayerForConnection " + success);

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
        mainMenuPanel.gameObject.SetActive(false);
        lobbyPanel.gameObject.SetActive(false);
        infoPanel.gameObject.SetActive(false);
    }
    #endregion

    #region ServerOverride
    //This hook is invoked when a server is started - including when a host is started.
    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
        currentMode = NetworkMode.Lobby;
        StartCoroutine("CheckLobbyReady");
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
                lobbyPlayerArray[i] = player.GetComponent<LobbyPlayer>();
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
                        if (dpcconnid == conn.connectionId)
                        {
                            isExistingPlayer = true;
                            Debug.Log(k + " existing " + dpcconnid);
                            disconnectedPlayerControllers[k] = null;
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
        ChangeTo(mainMenuPanel);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnClientError " + conn.connectionId);

        ChangeTo(mainMenuPanel);
        infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect " + conn.connectionId);

        base.OnClientConnect(conn);

        infoPanel.gameObject.SetActive(false);

        if (!NetworkServer.active)
        {//only to do on pure client (not self hosting client)
            ChangeTo(lobbyPanel);
            backDelegate = StopClientClbk;
            SetServerInfo("Client", networkAddress);
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
        base.OnStartHost();

        ChangeTo(lobbyPanel);
        backDelegate = StopHostClbk;
        SetServerInfo("Hosting", networkAddress);
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
}
