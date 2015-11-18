using UnityEngine;
using UnityEngine.UI;

public class ServerMissionPlayerInfo : MonoBehaviour {
    public Image roleImage;
    public Text RoleNameText;
    public Text rankText;
    public Slider randSlider;
    public Text rankPercent;
    public Text userNameText;
    public Sprite[] userPanelSprite;

	// Use this for initialization
	void Start () {
        
	}

    public void SetUserInfo(string username, PlayerRole pr, int rank, int experience, int fullEx)
    {
        SetUser(username, pr);
        rankText.text = "" + rank;
        rankPercent.text = "<color=#FFFFFFFF>" + experience + "</color>/" + fullEx;
        randSlider.value = ((float)experience) / fullEx;
    }

    private void SetUser(string username, PlayerRole pr)
    {
        if (pr == PlayerRole.Unselected) pr = PlayerRole.Striker;
        userNameText.text = username;
        RoleNameText.text = pr.ToString();
        switch (pr)
        {
            case PlayerRole.Striker:
                roleImage.sprite = userPanelSprite[0];
                break;
            case PlayerRole.Engineer:
                roleImage.sprite = userPanelSprite[1];
                break;
            case PlayerRole.Defender:
                roleImage.sprite = userPanelSprite[2];
                break;
        }
    }

    public void Reset()
    {
        SetUserInfo("", PlayerRole.Striker, 0, 10, 10);
    }
}
