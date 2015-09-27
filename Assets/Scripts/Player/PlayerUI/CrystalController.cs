using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrystalController : MonoBehaviour {

    public Image[] ultiCrystals;
    public Sprite[] crystalSprites;

    public void generateCrystal(int[] crystals) {
        for (int i = 0; i < ultiCrystals.Length; i++) {
            ultiCrystals[i].sprite = i >= crystals.Length ? null : crystalSprites[crystals[i]];
        }
    }

    public void voidCrystal() { 
        foreach(Image image in ultiCrystals){
            image.sprite = null;
        }
    }

}
