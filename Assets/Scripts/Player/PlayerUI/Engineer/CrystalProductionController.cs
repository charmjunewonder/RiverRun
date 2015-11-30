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

    public int GetCrystal() { return crystalSelected; }

    #region Animation
    public void TriggerAnimation() {
        if (!isFinished())
        {
            StartCoroutine("StartArrow");
            AudioController.Singleton.PlayEngineerCystalProductionSound();

        }
    }

    public void Revoke() {
        StopAllCoroutines();
        crystalSelected = -1;

        for (int i = 1; i < 5; i++)
        {
            Transform child = transform.GetChild(i);
            Image image = child.GetComponent<Image>();
            image.raycastTarget = false;
            image.color = new Color(1, 1, 1, 0);
            child.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        GetComponent<Image>().fillAmount = 0;
        arrows.GetComponent<Image>().fillAmount = 0;

    }

    public bool isFinished() {
        return transform.GetChild(4).GetComponent<Image>().color.a == 1;
    }
    #endregion

    #region Coroutines
    IEnumerator StartArrow()
    {
        Debug.Log("Coroutine");
        while (arrows.GetComponent<Image>().fillAmount < 1)
        {
            arrows.GetComponent<Image>().fillAmount += 0.08f;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine("StartBar");
    }

    IEnumerator StartBar()
    {
        while (GetComponent<Image>().fillAmount < 1)
        {
            GetComponent<Image>().fillAmount += 0.04f;
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 1; i <= 4; i++) {
            Image image = transform.GetChild(i).GetComponent<Image>();
            image.fillAmount = 1;
            image.raycastTarget = true;
            image.color = new Color(1, 1, 1, 1);
        }
    }

    
    #endregion
}
