using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerPlayerHealth : MonoBehaviour {

    public Sprite[] sprites;
    public Text healthPercent;

    public void setHealth(int health)
    {
        int num = 0;
        if (health <= 4)
        {
            num = 2;
            healthPercent.text = "<color=#B02E45FF>" + health * 10 + "%</color>";
        }
        for (int i = 0; i < 10; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = i < health ? sprites[num] : sprites[num + 1];
        }
    }
}
