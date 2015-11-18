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
    public int test;
	// Use this for initialization
	void Start () {
        //SetMissonComplete(100, 3, "Eric", PlayerRole.Engineer, 10, 60, 100, 15, 10);
	}


    public void SetMissonComplete(int score, int star, string username, PlayerRole pr, int rank, int experience, int fullEx, int skill1, int ulti)
    {
        scoreText.text = "" + score;
        SetStar(star);
        SetUser(username, pr);
        rankText.text = "" + rank;
        rankPercent.text = "<color=#FFFFFFFF>"+experience+"</color>/"+fullEx;
        randSlider.value = ((float)experience) / fullEx;
        skill1Text.text = "Defended <color=#B3F898FF>"+skill1+"</color> Enermy's Attacks";
        utilText.text = "Activate <color=#B3F898FF>" + ulti + "</color> Times Ultimate Skills";
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

    private void SetUser(string username, PlayerRole pr)
    {
        if (pr == PlayerRole.Unselected) pr = PlayerRole.Striker;
        userNameText.text = username;
        switch (pr)
        {
            case PlayerRole.Striker:
                userPanel.sprite = userPanelSprite[0];
                break;
            case PlayerRole.Engineer:
                userPanel.sprite = userPanelSprite[1];
                break;
            case PlayerRole.Defender:
                userPanel.sprite = userPanelSprite[2];
                break;
        }
    }
}
