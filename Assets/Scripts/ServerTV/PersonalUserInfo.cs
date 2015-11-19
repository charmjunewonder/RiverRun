using UnityEngine;
using UnityEngine.UI;

public class PersonalUserInfo : MonoBehaviour {
    public Text rankText;
    public Slider randSlider;
    public Text userNameText;
    public Text scoreText;

	// Use this for initialization
	void Start () {
	
	}

    public void SetRecord(string name, int score, int rank, int experience, int fullExp)
    {
        userNameText.text = name;
        rankText.text = ""+rank;
        randSlider.value = experience * 1.0f / fullExp;

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
