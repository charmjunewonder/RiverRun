using UnityEngine;
using System.Collections;

public class LeaderBoardPanel : MonoBehaviour {

    public GameObject teamPanel;
    public GameObject personalPanel;

    public LeaderBoardRecord[] teamRecords;
    public PersonalUserInfo[] personalRecords;
    public static LeaderBoardPanel Singleton;

	// Use this for initialization
	void Start () {
        Singleton = this;
        teamPanel.SetActive(false);
        personalPanel.SetActive(false);
	}

    public void ShowTeamPanel()
    {
        teamPanel.SetActive(true);
        personalPanel.SetActive(false);
        //foreach (LeaderBoardRecord lbr in teamRecords)
        //{
        //    lbr.SetRecord("Ruth", "Shirley", "Mike", "Ralph", 10301186);
        //}
        //teamRecords[0].SetRecord("Ruth", "Shirley", "Mike", "Ralph", 10301186);
        //teamRecords[1].SetRecord("Feiran", "Zhen", "Emily", "Eric", 9552186);
        //teamRecords[2].SetRecord("Brentt", "Kirsten", "Atit", "Joseph", 8243189);
        //teamRecords[3].SetRecord("Stephanie", "Laura", "Mike", "Ming", 6193328);
        //teamRecords[4].SetRecord("Ruth", "Shirley", "Mike", "Ralph", 5324819);
    }

    public void ShowPersonalPanel()
    {
        teamPanel.SetActive(false);
        personalPanel.SetActive(true);
        personalRecords[0].SetRecord("Eric", 1350, 13, 50, 100);
        personalRecords[1].SetRecord("Feiran", 2350, 15, 70, 100);
        personalRecords[2].SetRecord("Zhen", 3350, 16, 30, 100);

    }

    public void OnCloseClicked()
    {
        teamPanel.SetActive(false);
        personalPanel.SetActive(false);
    }
}
