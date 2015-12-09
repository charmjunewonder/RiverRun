using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerPlayerHealth : MonoBehaviour {

    public Sprite[] sprites;
    public Text healthPercent;

    public void setHealth(float health)
    {
        int num = 0;
        if (health <= 0.4f)
        {
            num = 2;
            healthPercent.text = "<color=#B02E45FF>" + ((int)(health * 100)).ToString() + "%</color>";
        }
        else
        {
            healthPercent.text = ((int)(health * 100)).ToString() + "%";
        }

        for (int i = 0; i < 10; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = i < health * 10 ? sprites[num] : sprites[num + 1];
        }
    }
}
