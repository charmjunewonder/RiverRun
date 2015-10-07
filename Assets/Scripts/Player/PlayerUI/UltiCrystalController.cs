﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UltiCrystalController : MonoBehaviour {

    public Image[] crystalImages;
    public GameObject cancelButton;
    public Sprite[] crystalSprites;
    public Sprite[] highlightedCrystalSprites;
    public Sprite transparent;

    private int[] crystals;
    private PlayerController playerController;

    public GameObject arrows;
    private bool startArrows, startBar, startCrystal;

    public void setPlayerController(PlayerController plc) { playerController = plc; }
    

    public void AcceptCrystal(int index)
    {
        for (int i = 0; i < crystals.Length; i++) {
            if (crystals[i] == index) {
                crystalImages[i].sprite = highlightedCrystalSprites[index];
                crystals[i] = -i - 1;
                if (CheckComplete()) {
                    playerController.ActivateUlti();
                }
                return;
            }
        }
        Clear();
    }

    public void Clear() {
        playerController.CmdUltiFailureHandling();
        for (int i = 0; i < crystals.Length; i++) {
            crystalImages[i].sprite = transparent;
            crystalImages[i].color = new Color(0, 0, 0, 0);
        }
        crystals = null;
        StartCoroutine("CloseBar");
    }

    public void Revoke() {
        Clear();
        playerController.RevokeUlti();
    }

    public void GenerateUltiCrystals(){
        StartCoroutine("StartArrow");
    }

    private void GenerateUltiCrystalHelper() {
        int len = (int)Random.Range(3.0f, 5.99f);
        crystals = new int[len];
        for (int i = 0; i < len; i++)
        {
            crystals[i] = (int)Random.Range(0.0f, 3.99f);
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
}
