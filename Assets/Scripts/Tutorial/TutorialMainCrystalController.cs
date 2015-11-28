using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialMainCrystalController : MonoBehaviour {

    public TutorialEngineerController teController;

    public Sprite[] crystalSprites;
    public Sprite[] portalSprites;

    public int current_slot, current_type;
    public Image draggedImage;

    void Start() {
        current_slot = -1;
        current_type = -1;
    }

    void Update() {
        if (current_slot != -1)
        {
#if UNITY_STANDALONE_WIN
            if (Input.GetMouseButton(0))
                draggedImage.transform.position = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
            {
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

    public void SetCrystal(int slot, int type) {
        transform.GetChild(slot).GetChild(0).GetComponent<Image>().sprite = crystalSprites[type];
        transform.GetChild(slot).GetChild(0).GetComponent<Image>().color = new Color(1,1,1,1);
    }

    public void SetPortal(bool open, string name) {
        if (open) {
            transform.GetChild(4).GetComponent<Image>().sprite = portalSprites[1];
            transform.GetChild(6).GetComponent<Text>().text = name;
        }
        else {
            transform.GetChild(4).GetComponent<Image>().sprite = portalSprites[0];
            transform.GetChild(6).GetComponent<Text>().text = "";
        }
    }

    public void SelectCrystal(int slot_num)
    {
        if (slot_num == 1 || slot_num == 2)
        {
            transform.GetChild(slot_num).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);

            current_slot = slot_num;
            current_type = slot_num;

            draggedImage.sprite = crystalSprites[slot_num];
            draggedImage.transform.position = Input.mousePosition;
            draggedImage.color = new Color(1, 1, 1, 1);

        }
    }


    private void checkValidation()
    {
        if (current_type != -1)
        {
            SetCrystal(current_slot, current_type);
            current_slot = -1;
            current_type = -1;
            draggedImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            return;
        }
    }

    public void SupportCrystal()
    {
        if (current_type != -1)
        {
            teController.CrystalSupported(current_type);
            current_slot = -1;
            current_type = -1;
            draggedImage.color = new Color(1, 1, 1, 0);

        }
    }
}
