using UnityEngine;
using System.Collections;

public class EngiSkill0Controller : SkillController {

    private bool isSkillSelected;

    public bool IsSkillSelected() {
        return isSkillSelected;
    }

    public void SelectedSkill() {
        transform.parent.GetComponent<EngiSkillSwitch>().SetSkill(0);
        isSkillSelected = true;
    }

    public void DeslectedSkill() {
        isSkillSelected = false;
    }

}
