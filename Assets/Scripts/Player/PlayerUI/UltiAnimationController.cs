using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UltiAnimationController : MonoBehaviour {

    private GameObject arrows;
    private bool startArrows, startBar, startCrystal;

    void Start() {
        arrows = transform.GetChild(0).gameObject;
        startArrows = false;
        startBar = false;
        startCrystal = false;
    }

    public void TriggerAnimation() {
        StartCoroutine("StartArrow");
    }

    void Update() {
        /*
        if (startArrows) {
            arrows.GetComponent<Image>().fillAmount += 0.1f;
            if (arrows.GetComponent<Image>().fillAmount >= 1) {
                startArrows = false;
                startBar = true;
            }
        }
        else if (startBar) {
            GetComponent<Image>().fillAmount += 0.1f;
            if (arrows.GetComponent<Image>().fillAmount >= 1) {
                startBar = false;
                GetComponent<UltiCrystalController>().GenerateUltiCrystals();
            }
        }
         */
    }




}
