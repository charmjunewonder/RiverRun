using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Network;

public enum NetworkMode { Lobby, Game };

public class NetworkManagerCustom : NetworkManager {
    public GameObject LobbyPlayerPrefab;
    public GameObject StrikerPrefab;

    public ArrayList gameplayerControllers { get; set; }
    public ArrayList lobbyPlayerArray { get; set; }
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
        for (int i = 0; i < maxPlayers; i++)
        {
            lobbyPlayerArray.Add(null);
            gameplayerControllers.Add(null);
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
        ChangeTo(mainMenuPanel);
    }

    public void StopHostClbk()
    {

        StopHost();
        ChangeTo(mainMenuPanel);
    }

    public void StopClientClbk()
    {
        StopClient();
        ChangeTo(mainMenuPanel);
    }

    public void StopServerClbk()
    {
        StopServer();
        ChangeTo(mainMenuPanel);
    }

    public void StopGameClbk()
    {
        //SendReturnToLobby();
        ChangeTo(lobbyPanel);
    }
    #endregion

    IEnumerator CheckLobbyReady()
    {
        while (true)
        {
            Debug.Log("CheckLobbyReady");
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
                
                bool success = NetworkServer.ReplacePlayerForConnection(conn, newPlayer, lp.GetComponent<LobbyPlayer>().playerControllerId);
                Debug.Log(success);
                Destroy(lp.gameObject);
            }
        }
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
        StartCoroutine("CheckLobbyReady");
    }

    //Called on the server when a new client connects.
    public override void OnServerConnect(NetworkConnection conn){
		
		int i = 0;
		for(; i < maxPlayers; i++){
			if(lobbyPlayerArray[i] == null){
				break;
			}
		}
		if(i >= maxPlayers) {
			conn.Disconnect();
		}
		//		slotArray[i] = conn.playerControllers[0].gameObject;
		//		conn.playerControllers[0].gameObject.GetComponent<Player_ID>().SetSlot(i);
	}

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Lobby add" );

        int i = 0;
        for (; i < maxPlayers; i++)
            if (lobbyPlayerArray[i] == null) break;
        if (i == maxPlayers) return;
        var player = (GameObject)GameObject.Instantiate(LobbyPlayerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<LobbyPlayer>().slot = i;
        lobbyPlayerArray[i] = player.GetComponent<LobbyPlayer>();
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
    #endregion

    #region ClientOverride
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ChangeTo(mainMenuPanel);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        ChangeTo(mainMenuPanel);
        infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
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
        DisableLobbyUI();
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
}
