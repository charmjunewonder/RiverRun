using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UltiCrystalController : MonoBehaviour {

    public Image[] crystalImages;
    public Sprite[] crystalSprites;
    public Sprite[] highlightedCrystalSprites;

    private int[] crystals;
    private PlayerController playerController;

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
        playerController.CmdUltiFailureHandling();
    }

    public void Clear() {
        for (int i = 0; i < crystals.Length; i++) {
            crystalImages[i].sprite = null;
        }
        crystals = null;
    }

    public void GenerateUltiCrystals()
    {
        int len = (int)Random.Range(3.0f, 5.99f);
        crystals = new int[len];
        for (int i = 0; i < len; i++)
        {
            crystals[i] = (int)Random.Range(0.0f, 3.99f);
            crystalImages[i].sprite = crystalSprites[crystals[i]];
        }
    }




    private bool CheckComplete() {
        for (int i = 0; i < crystals.Length; i++) {
            if (crystals[i] >= 0)
                return false;
        }
        return true;
    }



}
