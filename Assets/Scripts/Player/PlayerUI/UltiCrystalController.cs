using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UltiCrystalController : MonoBehaviour {

    public Image[] crystalImages;
    public GameObject cancelButton;
    public Sprite[] crystalSprites;
    public Sprite[] highlightedCrystalSprites;
    public Material highlightedMaterial;
    public Sprite transparent;

    private int[] crystals;
    private PlayerController playerController;

    public GameObject arrows;

    public void setPlayerController(PlayerController plc) { playerController = plc; }
    

    public void AcceptCrystal(int index)
    {
        for (int i = 0; i < crystals.Length; i++) {
            if (crystals[i] == index) {
                crystalImages[i].sprite = highlightedCrystalSprites[index];
                crystalImages[i].material = highlightedMaterial;
                crystals[i] = -i - 1;
                if (CheckComplete()) {
                    playerController.ActivateUlti();
                }
                return;
            }
        }
        playerController.UltiFailureHandling();
        Clear();
    }

    public void Clear() {
        
        for (int i = 0; i < crystals.Length; i++) {
            crystalImages[i].sprite = transparent;
            crystalImages[i].color = new Color(0, 0, 0, 0);
        }
        crystals = null;
        GetComponent<Image>().fillAmount = 0;
        cancelButton.SetActive(false);
    }

    public void Revoke() {
        playerController.RevokeUlti();
        Clear();
        
    }

    public void GenerateUltiCrystals(){
        GetComponent<Image>().fillAmount = 1;
        cancelButton.SetActive(true);
        GenerateUltiCrystalHelper();
    }

    private void GenerateUltiCrystalHelper() {
        int len = Random.Range(3, 6);
        crystals = new int[len];
        for (int i = 0; i < len; i++)
        {
            crystals[i] = Random.Range(0, 4);
            crystalImages[i].sprite = crystalSprites[crystals[i]];
            crystalImages[i].color = new Color(1, 1, 1, 1);
        }
    }


    private bool CheckComplete() {
        for (int i = 0; i < crystals.Length; i++) {
            if (crystals[i] >= 0)
                return false;
        }
        return true;
    }

    /*
    IEnumerator StartArrow(){
        while (arrows.GetComponent<Image>().fillAmount < 1)
        {
            arrows.GetComponent<Image>().fillAmount += 0.04f;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine("StartBar");
    }

    IEnumerator StartBar(){
        while (GetComponent<Image>().fillAmount < 1)
        {
            GetComponent<Image>().fillAmount += 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        GenerateUltiCrystalHelper();
        cancelButton.SetActive(true);
    }

    IEnumerator CloseBar() {
        cancelButton.SetActive(false);
        while (GetComponent<Image>().fillAmount > 0) {
            GetComponent<Image>().fillAmount -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine("CloseArrow");
    }

    IEnumerator CloseArrow() {
        while (arrows.GetComponent<Image>().fillAmount > 0) {
            arrows.GetComponent<Image>().fillAmount -= 0.04f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    */
}
