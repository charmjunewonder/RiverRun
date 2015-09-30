using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class GuiLobbyManager : NetworkLobbyManager
{
	public LobbyCanvasControl lobbyCanvas;
	public OfflineCanvasControl offlineCanvas;
	public OnlineCanvasControl onlineCanvas;
	public ExitToLobbyCanvasControl exitToLobbyCanvas;
	public ConnectingCanvasControl connectingCanvas;
	public PopupCanvasControl popupCanvas;
	public MatchMakerCanvasControl matchMakerCanvas;
	public JoinMatchCanvasControl joinMatchCanvas;
	public LevelInfoCanvasControl levelInfoCanvasControl;
	
	public string onlineStatus;
	static public GuiLobbyManager s_Singleton;
	
	public Roles ownRole;

    public ArrayList gameplayerControllers = null;

	void Start()
	{
		s_Singleton = this;
		
		Debug.Log("Lobby Manager: start");
		
		offlineCanvas.Show();
		//StartCoroutine(show1());
		//DontDestroyOnLoad(gameObject);
	}
	
	void Update(){
		//Debug.Log("Lobby Manager: start1");
		
		//offlineCanvas.Show();
		
	}

	


	void OnLevelWasLoaded()
	{
		if (lobbyCanvas != null) lobbyCanvas.OnLevelWasLoaded();
		if (offlineCanvas != null) offlineCanvas.OnLevelWasLoaded();
		if (onlineCanvas != null) onlineCanvas.OnLevelWasLoaded();
		if (exitToLobbyCanvas != null) exitToLobbyCanvas.OnLevelWasLoaded();
		if (connectingCanvas != null) connectingCanvas.OnLevelWasLoaded();
		if (popupCanvas != null) popupCanvas.OnLevelWasLoaded();
		if (matchMakerCanvas != null) matchMakerCanvas.OnLevelWasLoaded();
		if (joinMatchCanvas != null) joinMatchCanvas.OnLevelWasLoaded();
		if (levelInfoCanvasControl != null) levelInfoCanvasControl.OnLevelWasLoaded();
		
	}

	public void SetFocusToAddPlayerButton()
	{
		if (lobbyCanvas == null)
			return;

		lobbyCanvas.SetFocusToAddPlayerButton();
	}
	public override void OnLobbyStartHost(){
		Debug.Log("connect2");
		
	}
	
	// ----------------- Server callbacks ------------------

	public override void OnLobbyStopHost()
	{
		lobbyCanvas.Hide();
		Debug.Log("disconnect2");
		StartCoroutine(show1());
		
		//offlineCanvas.Show();
	}
	IEnumerator show1(){
		yield return new WaitForSeconds(0.2f);
		//while(true){
		offlineCanvas.Show();
		Debug.Log("show123");
		
		//yield return new WaitForSeconds(0.2f);
		
		//}	
	}
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
        if (gameplayerControllers == null){
            
            gameplayerControllers = new ArrayList(lobbySlots.Length);

            for (int i = 0; i < lobbySlots.Length; i++)
                gameplayerControllers.Add(null);

                Debug.Log("Lobby Manager   : " + gameplayerControllers.Count + " " + lobbyPlayer.GetComponent<PlayerLobby>().slot);
        }
        Debug.Log("Lobby Manager: " + gameplayerControllers.Count + " " + lobbyPlayer.GetComponent<PlayerLobby>().slot);
        gameplayerControllers[lobbyPlayer.GetComponent<PlayerLobby>().slot] = gamePlayer.GetComponent<PlayerController>();
        gamePlayer.GetComponent<PlayerController>().slot = lobbyPlayer.GetComponent<PlayerLobby>().slot;
		//This hook allows you to apply state data from the lobby-player to the game-player
		//var cc = lobbyPlayer.GetComponent<ColorControl>();
		//var playerX = gamePlayer.GetComponent<Player>();
		//playerX.myColor = cc.myColor;
		return true;
	}

	// ----------------- Client callbacks ------------------

	public override void OnLobbyClientConnect(NetworkConnection conn)
	{
		connectingCanvas.Hide();		
	}

	public override void OnClientError(NetworkConnection conn, int errorCode)
	{
		
		connectingCanvas.Hide();
		StopHost();

		popupCanvas.Show("Client Error", errorCode.ToString());
	}

	public override void OnLobbyClientDisconnect(NetworkConnection conn)
	{
		lobbyCanvas.Hide();
		Debug.Log("disconnect");
		offlineCanvas.Show();
	}

	public override void OnLobbyStartClient(NetworkClient client)
	{
		if (matchInfo != null)
		{
			connectingCanvas.Show(matchInfo.address);
		}
		else
		{
			connectingCanvas.Show(networkAddress);
		}
	}

	public override void OnLobbyClientAddPlayerFailed()
	{
		popupCanvas.Show("Error", "No more players allowed.");
	}

	public override void OnLobbyClientEnter()
	{
		lobbyCanvas.Show();
		onlineCanvas.Show(onlineStatus);
		
		exitToLobbyCanvas.Hide();
	}
	
	public override void OnLobbyClientExit()
	{
		lobbyCanvas.Hide();
		onlineCanvas.Hide();
		levelInfoCanvasControl.Hide();
		if (Application.loadedLevelName == base.playScene)
		{
			//exitToLobbyCanvas.Show();
		}
	}
	
//	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
//	{
//		
//		GameObject player = (GameObject)Instantiate(playerPrefab);
//		//player.GetComponent<Player>().color = Color.Red;
//		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
//	}
	
	public override void OnLobbyServerSceneChanged(string sceneName){
		levelInfoCanvasControl.Hide();
	}
	
	public void showLevelInfo(int level){
		levelInfoCanvasControl.Show(level);
	}
}
