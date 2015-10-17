using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EngiCrystaController : MonoBehaviour {

    public void Select() {
        
        Transform p = transform.parent;

        if(p.GetComponent<CrystalProductionController>().GetCrystal() != -1)
            return;

        for(int i = 1; i <= 4; i++){
           p.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void Deselect() {
        if (transform.parent.GetComponent<CrystalProductionController>().GetCrystal() != -1)
            return;
        transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        
    }

    public void Lock(int index) {
        Transform p = transform.parent;
        p.GetComponent<CrystalProductionController>().SelectCrystal(index);
        for (int i = 1; i <= 4; i++){
            p.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
}
