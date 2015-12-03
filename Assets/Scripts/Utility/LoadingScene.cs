using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class LoadingScene : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}

    public void GoToLobbyScene()
    {
        Application.LoadLevel("Lobby");
        //StartCoroutine(loadLevel("Lobby"));
    }


}
