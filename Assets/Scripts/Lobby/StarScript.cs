using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StarScript : MonoBehaviour {
    [SerializeField]
    Sprite[] stars;

    public void ShowStar()
    {
        GetComponent<Image>().sprite = stars[1];
    }

    public void UnshowStar()
    {
        GetComponent<Image>().sprite = stars[0];
    }
}
