using UnityEngine;
using System.Collections;

public class EngiSkill0Controller : SkillController {

    public ReminderController reminderController;

    private bool isSkillSelected;

    public bool IsSkillSelected() {
        return isSkillSelected;
    }

    public void SelectedSkill() {
        if (getCoolDownStatus()) {
            reminderController.setReminder("Skill is Cooling Down", 1);
            return;
        }
        transform.parent.GetComponent<EngiSkillSwitch>().SetSkill(0);
        isSkillSelected = true;
    }

    public void DeslectedSkill() {
        isSkillSelected = false;
    }

}
