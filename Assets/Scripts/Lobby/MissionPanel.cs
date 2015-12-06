using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MissionPanel : MonoBehaviour {

    [SerializeField] Image userPanel;
    [SerializeField] StarScript[] stars;
    [SerializeField] Text RoleNameText;
    [SerializeField] Text scoreText;
    [SerializeField] Text rankText;
    [SerializeField] Slider randSlider;
    [SerializeField] Text rankPercent;
    [SerializeField] Text userNameText;
    [SerializeField] Sprite[] userPanelSprite;
    [SerializeField]
    Text skill1Text;
    [SerializeField]
    Text utilText;
    [SerializeField]
    Text supportText;
    public int test;
	// Use this for initialization
	void Start () {
        //SetMissonComplete(100, 3, "Eric", PlayerRole.Engineer, 10, 60, 100, 15, 10);
        //StartCoroutine(starsss());
	}

    IEnumerator starsss()
    {
        while (true)
        {
            int ssss = (int)Random.Range(0, 300);
            SetStar(ScoreParameter.CalcuateStar(ssss));
            scoreText.text = "" + ssss;

            yield return new WaitForSeconds(1);
        }
    }

    public void SetMissonComplete(int score, int star, string username, PlayerRole pr, int rank, int experience, int fullEx, int skill1, int ulti, int support)
    {
        scoreText.text = "" + score;
        SetStar(star);
        rankText.text = "" + rank;
        rankPercent.text = "<color=#FFFFFFFF>"+experience+"</color>/"+fullEx;
        randSlider.value = ((float)experience) / fullEx;


        if (pr == PlayerRole.Unselected) pr = PlayerRole.Striker;
        userNameText.text = username;
        switch (pr)
        {
            case PlayerRole.Striker:
                userPanel.sprite = userPanelSprite[0];
                skill1Text.text = "Attacked <color=#B3F898FF>" + skill1 + "</color> Times.";
                utilText.text = "Activated <color=#B3F898FF>" + ulti + "</color> Times Ultimate Skills.";
                supportText.text = "Supported Teammates <color=#B3F898FF>" + support + "</color> Times.";
                break;
            case PlayerRole.Engineer:
                userPanel.sprite = userPanelSprite[1];
                skill1Text.text = "Recovered Teammates <color=#B3F898FF>" + skill1 + "</color> Times.";
                utilText.text = "Generated <color=#B3F898FF>" + ulti + "</color> Crystals.";
                supportText.text = "Supported Teammates <color=#B3F898FF>" + support + "</color> Times.";
                break;
            case PlayerRole.Defender:
                userPanel.sprite = userPanelSprite[2];
                skill1Text.text = "Defended <color=#B3F898FF>" + skill1 + "</color> Enemy's Attacks.";
                utilText.text = "Activated <color=#B3F898FF>" + ulti + "</color> Times Ultimate Skills.";
                supportText.text = "Supported Teammates <color=#B3F898FF>" + support + "</color> Times.";
                break;                
        }
    }

    private void SetStar(int star)
    {
        if (star < 0) star = 0;
        if (star > 4) star = 4;
        for (int i = 0; i < 5; i++)
        {
            stars[i].UnshowStar();
        }
        for (int i = 0; i <= star; i++)
        {
            stars[i].ShowStar();
        }
    }

    public void OnContinueClicked()
    {
        
        NetworkManagerCustom.SingletonNM.StopClientClbk();
        gameObject.SetActive(false);
    }
}
