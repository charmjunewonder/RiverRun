using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EngiSkill0Controller : SkillController {

    public ReminderController reminderController;
    public Material selectedMaterial;

    private bool isSkillSelected;

    public bool IsSkillSelected() {
        return isSkillSelected;
    }

    public void SelectedSkill() {
        if (getCoolDownStatus()) {
            reminderController.setReminder("Skill is Cooling Down", 1);
            return;
        }
        GetComponent<Image>().material = selectedMaterial;
        transform.parent.GetComponent<EngiSkillSwitch>().SetSkill(0);
        isSkillSelected = true;
    }

    public void DeslectedSkill() {
        isSkillSelected = false;
        GetComponent<Image>().material = null;
    }

}
