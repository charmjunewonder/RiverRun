using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainCrystalController : MonoBehaviour {

    public Sprite[] crystalSprites;
    public Sprite[] crystalPortalSprites;

    private PlayerController playerController;
    private int[] crystals;

    public int current_slot, current_type;
    private Image draggedImage;

    public void SetPlayerController(PlayerController p) { playerController = p; }

    void Start(){
        crystals = new int[4];
        for (int i = 0; i < 4; i++)
            crystals[i] = -1;

        current_slot = -1;
        current_type = -1;

        draggedImage = transform.GetChild(5).GetComponent<Image>();
    }

    void Update() {
        if (current_slot != -1) {
			#if UNITY_STANDALONE_WIN
            if(Input.GetMouseButton(0))
                draggedImage.transform.position = Input.mousePosition;
            if (Input.GetMouseButtonUp(0)) {
                Invoke("checkValidation", 0.1f);
            }
			#endif
			#if UNITY_IOS
			if(Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved) || (Input.GetTouch(0).phase == TouchPhase.Stationary))
				draggedImage.transform.position = Input.mousePosition;
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
				Invoke("checkValidation", 0.1f);
			}
			#endif
        }
    }

    private void checkValidation() {
        if (current_type != -1) {
            SetCrystal(current_slot, current_type);
            current_slot = -1;
            current_type = -1;
            draggedImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            return;
        }
    }

    public void AcceptCrystal(int crys_num) {

        bool flag = false;

        for (int i = 0; i < 4; i++) {
            if (crystals[i] == -1) {
                crystals[i] = crys_num;
                
                Image image = transform.GetChild(i).GetChild(0).GetComponent<Image>();
                image.sprite = crystalSprites[crys_num];
                image.color = new Color(1, 1, 1, 1);
                flag = true;
                break;
            }
        }

        if (!flag) {
            for (int i = 3; i > 0; i--) {
                crystals[i] = crystals[i-1];
                transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = crystalSprites[crystals[i]];
            }
            crystals[0] = crys_num;
            transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = crystalSprites[crys_num];
        }

        playerController.CmdUpdateDisconnectionCrystal(crystals[0] + 1, crystals[1] + 1, crystals[2] + 1, crystals[3] + 1);
    }

    public void SelectCrystal(int slot_num) {
        if (crystals[slot_num] != -1) {
            playerController.setDraggingCrystal(true);

            transform.GetChild(slot_num).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            
            current_slot = slot_num;
            current_type = crystals[slot_num];

            draggedImage.sprite = crystalSprites[current_type];
            draggedImage.transform.position = Input.mousePosition;
            draggedImage.color = new Color(1, 1, 1, 1);

            crystals[slot_num] = -1;

        }
    }

    public void SupportCrystal() {
        if (current_type != -1)
        {
            playerController.CmdSupport(current_type);
          
            current_slot = -1;
            current_type = -1;
            draggedImage.color = new Color(1, 1, 1, 0);

            playerController.CmdUpdateDisconnectionCrystal(crystals[0] + 1, crystals[1] + 1, crystals[2] + 1, crystals[3] + 1);
        }
    }

    public void SetCrystal(int slot_num, int crys_num) {
        crystals[slot_num] = crys_num;

        if (crys_num >= 0) {
            Image image = transform.GetChild(slot_num).GetChild(0).GetComponent<Image>();
            image.sprite = crystalSprites[crys_num];
            image.color = new Color(1, 1, 1, 1);
        }
        else {
            Image image = transform.GetChild(slot_num).GetChild(0).GetComponent<Image>();
            image.color = new Color(1, 1, 1, 0);
        }
    }

    public void PortalShine() {
        StopAllCoroutines();
        StartCoroutine("FadeShiningColor");
    }

    public void OpenCrystalPortal(string name) {
        transform.GetChild(4).GetComponent<Image>().sprite = crystalPortalSprites[1];
        transform.GetChild(6).GetComponent<Text>().text = name;
    }

    public void CloseCrystalPortal() {
        transform.GetChild(4).GetComponent<Image>().sprite = crystalPortalSprites[0];
        transform.GetChild(6).GetComponent<Text>().text = "";
    }

    IEnumerator FadeShiningColor() {
        Image image = transform.GetChild(4).GetChild(0).GetComponent<Image>();
        image.color = new Color(1, 1, 1, 1);
        while (image.color.a >= 0) {
            image.color -= new Color(0, 0, 0, 0.04f);
            yield return new WaitForSeconds(0.02f);
        }
        image.color = new Color(1, 1, 1, 0);
    }
}
