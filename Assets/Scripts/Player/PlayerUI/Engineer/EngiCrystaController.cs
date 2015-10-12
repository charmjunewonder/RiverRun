using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EngiCrystaController : MonoBehaviour {

    public void Select() {
        
        Transform p = transform.parent;

        for(int i = 1; i <= 4; i++){
            transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
}
