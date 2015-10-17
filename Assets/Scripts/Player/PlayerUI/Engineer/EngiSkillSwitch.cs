using UnityEngine;
using System.Collections;

public class EngiSkillSwitch : MonoBehaviour {

    private int skillSelected = 0;

    public void SetSkill(int index) {
        if (index == skillSelected)
            return;
        skillSelected = index;
        if (index == 0) {
            transform.GetChild(1).GetComponent<EngiSkill1Controller>().Revoke();
        } else if (index == 1) {
            transform.GetChild(0).GetComponent<EngiSkill0Controller>().DeslectedSkill();
        }
    }
}
