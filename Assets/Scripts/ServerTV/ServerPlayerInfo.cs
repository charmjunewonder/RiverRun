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
        SetUserInfo("Ericcc", PlayerRole.Defender, 40, 100);
    }
    public void SetUserInfo(string userName, PlayerRole pr, int health, int fullHealth)
    {
        userNameText.text = userName;
        roleNameText.text = pr.ToString();
        int percent = health * 10 / fullHealth;
        hc.setHealth(percent);
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
}
