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
        teamScore.text = "Team Score: " + score;
        int seconds = time % 60;
        int minutes = time / 60;
        string minutesText = "" + minutes;
        string secondsText = "" + seconds;
        if (minutes < 10) minutesText = "0" + minutesText;
        if (seconds < 10) secondsText = "0" + secondsText;
        timeText.text = "" + minutesText + "'" + secondsText + "''";
        protectText.text = "Condition: " + protectCondition;

        ArrayList gc = NetworkManagerCustom.SingletonNM.gameplayerControllers;
        for (int i = 0; i < NetworkManagerCustom.SingletonNM.maxPlayers; i++)
        {
            PlayerController lp = (PlayerController)gc[i];
            if (lp != null)
            {
                Debug.Log("SetServerMissionCompletePanel");
                playerInfos[i].gameObject.SetActive(true);
                playerInfos[i].SetUserInfo(lp.username, lp.role, lp.rank, 50, 100);
            }
            else
            {
                DisconnectedPlayerController dpc = (DisconnectedPlayerController)NetworkManagerCustom.SingletonNM.disconnectedPlayerControllers[i];
                if (dpc != null)
                {
                    playerInfos[i].gameObject.SetActive(true);
                    playerInfos[i].SetUserInfo(dpc.username, dpc.currentRole, 3, 100, 200);
                }
            }
        }
    }

    public void Reset()
    {
        teamScore.text = "Team Score: ";
        timeText.text = "";
        protectText.text = "Condition: Good";
        foreach (ServerMissionPlayerInfo spi in playerInfos)
        {
            if (spi != null)
            {
                spi.Reset();
                spi.gameObject.SetActive(false);
            }
        }
    }
}
