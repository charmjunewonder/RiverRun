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

        scoreText.text = System.String.Format("{0:n0}", score);
    }
}
