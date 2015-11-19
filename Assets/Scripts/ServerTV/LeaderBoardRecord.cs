using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardRecord : MonoBehaviour {
    public Text[] playerName;
    public Text scoreText;

	// Use this for initialization
	void Start () {
	
	}

    public void SetRecord(string name1, string name2, string name3, string name4, int score)
    {
        playerName[0].text = name1;
        playerName[1].text = name2;
        playerName[2].text = name3;
        playerName[3].text = name4;

        string scoreString = "";
        while (score != 0)
        {
            
            int temp = score % 1000;
            score /= 1000;
            scoreString = temp + "," + scoreString;
            Debug.Log(scoreString);
        }
        scoreText.text = scoreString;
    }
}
