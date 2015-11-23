using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBarController : MonoBehaviour {

    public void SetProgress(float perc) {
        transform.GetChild(1).GetComponent<Image>().fillAmount = perc;
        transform.GetChild(1).GetChild(0).GetComponent<Text>().text = ((int)(perc * 100)).ToString() + "%";
    }
}
