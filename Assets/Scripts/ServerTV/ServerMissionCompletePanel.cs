using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerMissionCompletePanel : MonoBehaviour {
    public Text teamScore;
    public Text timeText;
    public Text protectText;
    public ServerMissionPlayerInfo[] playerInfos;
	// Use this for initialization
	void Start () {
	
	}

    public void SetServerMissionCompletePanel(int score, int time, string protectCondition)
    {
        teamScore.text = "" + score;
        int seconds = time % 60;
        int minutes = time / 60;
        string minutesText = "" + minutes;
        string secondsText = "" + seconds;
        if (minutes < 10) minutesText = "0" + minutesText;
        if (seconds < 10) secondsText = "0" + secondsText;
        timeText.text = "" + minutesText + "'" + secondsText + "''";
        protectText.text = "Condition: " + protectCondition;

        ArrayList gc = NetworkManagerCustom.SingletonNM.gameplayerControllers;
    }
}
