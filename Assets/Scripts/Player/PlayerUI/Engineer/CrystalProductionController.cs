using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrystalProductionController : MonoBehaviour {

    public GameObject arrows;

    private EngineerController engineerController;

    private int crystalSelected;

    void Start() {
        crystalSelected = -1;
    }

    public void setEngineerConrtroller(EngineerController ec) {
        engineerController = ec;
    }

    public void SelectCrystal(int index) {
        crystalSelected = index;
    }

    #region Animation
    public void TriggerAnimation() {
        if(!isFinished())
            StartCoroutine("StartArrow");
    }

    public void Revoke() {
        StopAllCoroutines();
        for (int i = 0; i < 5; i++) {
            Image subImage = transform.GetChild(i).GetComponent<Image>();
            subImage.raycastTarget = false;
        }
        crystalSelected = -1;
        StartCoroutine("CloseBar");
    }

    public bool isFinished() {
        return transform.GetChild(4).GetComponent<Image>().fillAmount == 1;
    }
    #endregion

    #region Coroutines
    IEnumerator StartArrow()
    {
        while (arrows.GetComponent<Image>().fillAmount < 1)
        {
            arrows.GetComponent<Image>().fillAmount += 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine("StartBar");
    }

    IEnumerator StartBar()
    {
        while (GetComponent<Image>().fillAmount < 1)
        {
            GetComponent<Image>().fillAmount += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 1; i <= 4; i++) {
            Image image = transform.GetChild(i).GetComponent<Image>();
            image.fillAmount = 1;
            image.raycastTarget = true;
        }
    }

    IEnumerator CloseBar()
    {
        while (GetComponent<Image>().fillAmount > 0)
        {
            GetComponent<Image>().fillAmount -= 0.04f;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine("CloseArrow");
    }

    IEnumerator CloseArrow()
    {
        while (arrows.GetComponent<Image>().fillAmount > 0)
        {
            arrows.GetComponent<Image>().fillAmount -= 0.08f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    #endregion
}
