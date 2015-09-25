using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager {

	string ipAddress = "localhost";
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
	
}
