using UnityEngine;
using System.Collections;

public class LobbyTutorialController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public void GoToToturialScene()
    {
        GameObject[] lobbies = GameObject.FindGameObjectsWithTag("LobbyDelete");
        foreach(GameObject go in lobbies){
            Destroy(go);
        }
        Application.LoadLevel("Tutorial");
    }

    public void GoToLobbyScene()
    {
        Application.LoadLevel("Lobby");
    }
}
