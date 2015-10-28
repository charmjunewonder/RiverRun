using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarningController : MonoBehaviour {

    public void TriggerWarning() {
        StopAllCoroutines();
        StartCoroutine("Warning");
    }

    IEnumerator Warning() {
        GetComponent<Image>().color = new Color(1,1,1,1);
        float alpha = 1;
        while (alpha >= 0) {
            alpha -= 0.1f;
            GetComponent<Image>().color = new Color(1,1,1,alpha);
            yield return new WaitForSeconds(0.1f);
        }
        GetComponent<Image>().color = new Color(1, 1, 1, 0); 

    }
}
