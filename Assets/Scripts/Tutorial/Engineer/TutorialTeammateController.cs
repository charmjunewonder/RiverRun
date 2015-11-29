using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialTeammateController : MonoBehaviour {

    public CrystalProductionController cpController;

    public void Highlight()
    {
        if(transform.GetChild(4).GetComponent<Image>().color.a != 0 || cpController.isFinished())
            transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void NoHighlight()
    {
        transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

}
