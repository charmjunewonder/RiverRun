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
                skill1Text.text = "Attacked <color=#B3F898FF>" + skill1 + "</color> Enemy's Spaceships.";
                utilText.text = "Activated <color=#B3F898FF>" + ulti + "</color> Times Ultimate Skills.";
                break;
            case PlayerRole.Engineer:
                userPanel.sprite = userPanelSprite[1];
                skill1Text.text = "Recovered Teammates <color=#B3F898FF>" + skill1 + "</color> Times.";
                utilText.text = "Generated <color=#B3F898FF>" + ulti + "</color> Crystals.";
                break;
            case PlayerRole.Defender:
                userPanel.sprite = userPanelSprite[2];
                skill1Text.text = "Defended <color=#B3F898FF>" + skill1 + "</color> Enemy's Attacks.";
                utilText.text = "Activated <color=#B3F898FF>" + ulti + "</color> Times Ultimate Skills.";
                break;
            default:
                supportText.text = "Support Teammates <color=#B3F898FF>" + support + "</color> Times.";
                break;
        }
    }

    private void SetStar(int star)
    {
        if (star < 1) star = 1;
        if (star > 5) star = 5;
        for (int i = 0; i < 4; i++)
        {
            stars[i].UnshowStar();
        }
        for (int i = 0; i <= star - 1; i++)
        {
            stars[i].ShowStar();
        }
    }

}
