using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialTeammatePanelController : MonoBehaviour {

    public void SetGlow()
    {
        for (int i = 0; i < 4; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<Image>().color.a != 0)
            {
                child.GetChild(4).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void NoGlow()
    {
        for (int i = 0; i < 4; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<Image>().color.a != 0)
            {
                child.GetChild(4).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
    }
}
