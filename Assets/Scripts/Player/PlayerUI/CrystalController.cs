using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrystalController : MonoBehaviour {

    public int crystal_slot_num;

    public void Clicked() {
        transform.parent.GetComponent<MainCrystalController>().SelectCrystal(crystal_slot_num);
    }

}
