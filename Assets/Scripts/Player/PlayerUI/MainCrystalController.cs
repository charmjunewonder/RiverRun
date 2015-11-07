using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainCrystalController : MonoBehaviour {

    public Sprite[] crystalSprites;

    private PlayerController playerController;
    private int[] crystals;

    public void SetPlayerController(PlayerController p) { playerController = p; }

    void Start(){
        crystals = new int[4];
        for (int i = 0; i < 4; i++)
            crystals[i] = -1;
    }

    public void AcceptCrystal(int crys_num) {
        for (int i = 0; i < 4; i++) {
            if (crystals[i] == -1) {
                crystals[i] = crys_num;
                //playerController.AddCrystalToList(i, crys_num);

                Image image = transform.GetChild(i).GetChild(0).GetComponent<Image>();
                image.sprite = crystalSprites[crys_num];
                image.color = new Color(1, 1, 1, 1);

                break;
            }
        }
    }

    public void SupportCrystal(int slot_num) {
        if (crystals[slot_num] != -1) {
            playerController.CmdSupport(crystals[slot_num]);
            //playerController.RemoveCrystalFromList(slot_num);

            Image image = transform.GetChild(slot_num).GetChild(0).GetComponent<Image>();
            crystals[slot_num] = -1;
            image.color = new Color(1, 1, 1, 0);
        }
    }

    public void SetCrystal(int slot_num, int crys_num) {
        crystals[slot_num] = crys_num;

        Image image = transform.GetChild(slot_num).GetChild(0).GetComponent<Image>();
        image.sprite = crystalSprites[crys_num];
        image.color = new Color(1, 1, 1, 1);
    }

}
