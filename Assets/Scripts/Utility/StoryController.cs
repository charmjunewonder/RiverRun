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
#if UNITY_IOS
        Invoke("GoToLobbyScene", 1.5f);
#endif
#if UNITY_STANDALONE_WIN
        Invoke("GoToOpeningScene", 1.5f);
#endif
	}

    void ShowStoryPanel()
    {
        startPanel.SetActive(false);
        storyPanel.SetActive(true);
        Invoke("GoToLobbyScene", 10);
    }

    public void GoToOpeningScene()
    {
        Application.LoadLevel("Opening");
    }

    public void GoToLobbyScene()
    {
        Application.LoadLevel("Lobby");
    }
}
