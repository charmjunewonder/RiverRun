using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager {
    public ArrayList gameplayerControllers;

    static public NetworkManagerCustom s_singleton;
    void Start()
    {
        s_singleton = this;
        gameplayerControllers = new ArrayList(4);
        for (int i = 0; i < 4; i++)
        {
            gameplayerControllers.Add(null);
        }
    }
/*	string ipAddress = "localhost";
	public void StartupHost()
	{
		SetPort();
		NetworkManager.singleton.StartServer();
	}
	
	public void StartupClient()
	{
		SetIPAddress();
		SetPort();
		NetworkManager.singleton.StartClient();
	}
	
	void SetPort()
	{
		NetworkManager.singleton.networkPort = 7777;
	}
	
	void SetIPAddress()
	{
		//NetworkManager.singleton.networkAddress = "128.2.236.211";
		NetworkManager.singleton.networkAddress = ipAddress;
	}
	
	void OnLevelWasLoaded (int level)
	{
		if(level == 0)
		{
			SetupMenuSceneButtons();
		}
		
		else
		{
			//SetupOtherSceneButtons();
			

		}
	}
	
	void SetupMenuSceneButtons()
	{
		GameObject.Find("HostButton").GetComponent<Button>().onClick.RemoveAllListeners();
		GameObject.Find("HostButton").GetComponent<Button>().onClick.AddListener(StartupHost);
		
		GameObject.Find("ClientButton").GetComponent<Button>().onClick.RemoveAllListeners();
		GameObject.Find("ClientButton").GetComponent<Button>().onClick.AddListener(StartupClient);
	}
	
	void SetupOtherSceneButtons()
	{
		GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.RemoveAllListeners();
		GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);
	}
    */
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        int i = 0;
        for (; i < 4; i++)
        {
            if (gameplayerControllers[i] == null) break;
        }
        if (i == 4) return;
        var player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<PlayerController>().SetSlot(i);
        gameplayerControllers[i] = player.GetComponent<PlayerController>();
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }	 
}
