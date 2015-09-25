using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.EventSystems;

public enum Roles { Striker, Defender, Engineer, Simoner };

public class PlayerLobby : NetworkLobbyPlayer
{
	[SyncVar] private bool isLevelSelected = false;
	public Canvas playerCanvasPrefab;
	private Canvas playerCanvas;
	
	public Canvas levelCanvasPrefab;
	public Canvas levelCanvas;
	
	private Roles ownRole;
	// cached components
	ColorControl cc;
	NetworkLobbyPlayer lobbyPlayer;

	void Awake()
	{
		cc = GetComponent<ColorControl>();
		lobbyPlayer = GetComponent<NetworkLobbyPlayer>();
	}

	public override void OnClientEnterLobby()
	{
		CreatePlayerCanvas();

		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.SetReady(lobbyPlayer.readyToBegin);

		EventSystem.current.SetSelectedGameObject(hooks.colorButton.gameObject);
		
		if(slot == 0){
			if(isLevelSelected) return;
			if (levelCanvas == null)
			{
				levelCanvas = (Canvas)Instantiate(levelCanvasPrefab, Vector3.zero, Quaternion.identity);
				levelCanvas.sortingOrder = 10;
			}
			Debug.Log("dfjsl2");
			
			var levelHooks = levelCanvas.GetComponent<LevelCanvasHooks>();
			levelHooks.pLobby = this;
			Debug.Log(isLocalPlayer);
			Debug.Log(isServer);
			
			levelHooks.ShowInfo();
		}
	}

	public override void OnClientExitLobby()
	{
		if (playerCanvas != null)
		{
			Destroy(playerCanvas.gameObject);
		}
	}

	public override void OnClientReady(bool readyState)
	{
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.SetReady(readyState);
	}

	float GetPlayerPos(int slot)
	{
		var lobby = NetworkManager.singleton as GuiLobbyManager;
		if (lobby == null)
		{
			// no lobby?
			return slot * 200;
		}

		// this spreads the player canvas panels out across the screen
		var screenWidth = playerCanvas.pixelRect.width;
		screenWidth -= 200; // border padding
		var playerWidth = screenWidth / (lobby.maxPlayers-1);
		return -(screenWidth / 2) + slot * playerWidth;
	}

	public override void OnStartLocalPlayer()
	{
		CreatePlayerCanvas();

		// setup button hooks
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();

		hooks.OnColorChangeHook = OnGUIColorChange;
		hooks.OnReadyHook = OnGUIReady;
		hooks.OnRemoveHook = OnGUIRemove;
		
		hooks.OnChooseStriker = ChooseStriker;
		
		hooks.SetLocalPlayer();

		if(slot == 0){
			if(isLevelSelected) return;
			
			if (levelCanvas == null)
			{
				levelCanvas = (Canvas)Instantiate(levelCanvasPrefab, Vector3.zero, Quaternion.identity);
				levelCanvas.sortingOrder = 10;
			}
			Debug.Log("dfjsl2");
			
			var levelHooks = levelCanvas.GetComponent<LevelCanvasHooks>();
			levelHooks.pLobby = this;

			
			levelHooks.ShowLevel();
		}
	}
	
	public override void OnStartServer(){
		CreatePlayerCanvas();
	}
	
	public void ChooseStriker()
	{
		CmdChooseStriker();	
	}
	
	[Command]
	public void CmdChooseStriker()
	{
		ownRole = Roles.Striker;
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.ChooseRole(ownRole);
		RpcChooseStriker();
	}
	
	[ClientRpc]
	public void RpcChooseStriker(){
		ownRole = Roles.Striker;
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.ChooseRole(ownRole);
		OnGUIReady();
		if (isLocalPlayer){
			var lobby = GuiLobbyManager.singleton as GuiLobbyManager;
			lobby.ownRole = Roles.Striker;
		}	
	}
	
	public void ChooseDefender()
	{
		ownRole = Roles.Defender;
		
	}
	
	public void ChooseEngineer()
	{
		ownRole = Roles.Engineer;
		
	}
	
	public void ChooseSimoner()
	{
		ownRole = Roles.Simoner;
		
	}

	[Command]
	public void CmdChooseLevel(int level){
		var lobby = GuiLobbyManager.singleton as GuiLobbyManager;
		lobby.playScene = "Level" + level;
		lobby.showLevelInfo(level);
		RpcShowLevel(level);
		isLevelSelected = true;
	}
	
	[ClientRpc]
	public void RpcShowLevel(int level){
		Debug.Log("dfjsl");
		Debug.Log(levelCanvas);
		var lobby = GuiLobbyManager.singleton as GuiLobbyManager;
		lobby.playScene = "Level1";
		lobby.showLevelInfo(1);
		
		levelCanvas.gameObject.SetActive(false);
		
	}
	
	void OnDestroy()
	{
		if (playerCanvas != null)
		{
			Destroy(playerCanvas.gameObject);
		}
	}

	public void SetColor(Color color)
	{
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.SetColor(color);
	}

	public void SetReady(bool ready)
	{
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.SetReady(ready);
	}

	[Command]
	public void CmdExitToLobby()
	{
		var lobby = NetworkManager.singleton as GuiLobbyManager;
		if (lobby != null)
		{
			lobby.ServerReturnToLobby();
		}
	}

	// events from UI system

	void OnGUIColorChange()
	{
		Debug.Log("dfkl 1");
		
		if (isLocalPlayer){
			Debug.Log("dfkl 2");
			
			cc.ClientChangeColor();
		}
	}

	void OnGUIReady()
	{
		if (isLocalPlayer)
			lobbyPlayer.SendReadyToBeginMessage();
	}

	void OnGUIRemove()
	{
		if (isLocalPlayer)
		{
			ClientScene.RemovePlayer(lobbyPlayer.playerControllerId);

			var lobby = NetworkManager.singleton as GuiLobbyManager;
			if (lobby != null)
			{
				lobby.SetFocusToAddPlayerButton();
			}
		}
	}
	
	
	private void CreatePlayerCanvas(){
		if (playerCanvas == null)
		{
			playerCanvas = (Canvas)Instantiate(playerCanvasPrefab, Vector3.zero, Quaternion.identity);
			playerCanvas.sortingOrder = 1;
		}
		
		// setup button hooks
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.panelPos.localPosition = new Vector3(GetPlayerPos(lobbyPlayer.slot), 0, 0);
		hooks.SetColor(cc.myColor);
	}
}

