using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeammatePanelController : MonoBehaviour {

    public GameObject citizenshipGlow;

    public void SetGlow() {

        for (int i = 0; i < 4; i++) {
            DisableCrystalCoolDown();
            Transform child = transform.GetChild(i);
            if(child.GetComponent<Image>().color.a != 0)
            {
                child.GetChild(4).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
        citizenshipGlow.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void NoGlow() {
        Debug.Log("No Glow");
        for (int i = 0; i < 4; i++){
            Transform child = transform.GetChild(i);
            if(child.GetComponent<Image>().color.a != 0)
            {
                child.GetChild(4).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
        citizenshipGlow.GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    public void DisableCrystalCoolDown() {
        Debug.Log("Disable Crystal Cool Down");
        for (int i = 0; i < 4; i++) {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<Image>().color.a != 0)
            {
                child.GetComponent<EngiTeammateController>().setShowCoolDown(false);
                
            }
        }
    }

    public void EnableCrystalCoolDown()
    {
        Debug.Log("Enable Crystal Cool Down");
        for (int i = 0; i < 4; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponent<Image>().color.a != 0)
            {
                child.GetComponent<EngiTeammateController>().setShowCoolDown(true);
            }
        }
    }

}
