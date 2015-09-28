using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrystalController : MonoBehaviour {

    private PlayerController playerController;

    public Image image;

    public int crystalIndex;

    void Start() {
    
    }


    public void setPlayerController(PlayerController plc) { playerController = plc; }

    public void Clicked() {
        if (crystalIndex >= 0) {
            playerController.CmdSupport(crystalIndex);
            crystalIndex = -1;
            image.enabled = false;
        }
    }

}
