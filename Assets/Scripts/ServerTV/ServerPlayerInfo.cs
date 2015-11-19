using UnityEngine;
using UnityEngine.UI;

public class ServerPlayerInfo : MonoBehaviour {
    public Text userNameText;
    public Text roleNameText;
    public Image roleImage;
    public Sprite[] roleImageSet;
    public ServerPlayerHealth hc;
    void Start()
    {
        //SetUserInfo("Ericcc", PlayerRole.Defender, 40, 100);
    }
    public void SetUserInfo(string userName, PlayerRole pr, int health)
    {
        userNameText.text = userName;
        roleNameText.text = pr.ToString();
        hc.setHealth(health);
        switch (pr)
        {
            case PlayerRole.Striker:
                roleImage.sprite = roleImageSet[0];
                break;
            case PlayerRole.Engineer:
                roleImage.sprite = roleImageSet[1];
                break;
            case PlayerRole.Defender:
                roleImage.sprite = roleImageSet[2];
                break;
        }
    }

    public void SetHealth(int health)
    {
        hc.setHealth(health);
    }

    public void Reset()
    {
        SetUserInfo("", PlayerRole.Striker, 10);
    }
}
