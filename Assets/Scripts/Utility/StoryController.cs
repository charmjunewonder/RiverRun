using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StoryController : MonoBehaviour {
    public GameObject storyPanel;
    public GameObject startPanel;

	// Use this for initialization
	void Start () {
        startPanel.SetActive(true);
        storyPanel.SetActive(false);
//#if UNITY_IOS
        Invoke("GoToLobbyScene", 2);
//#endif
//#if UNITY_STANDALONE_WIN
//        Invoke("ShowStoryPanel", 2);
//#endif
	}

    void ShowStoryPanel()
    {
        startPanel.SetActive(false);
        storyPanel.SetActive(true);
        Invoke("GoToLobbyScene", 10);
    }

    public void GoToLobbyScene()
    {
        Application.LoadLevel("Lobby");
    }
}
