using UnityEngine;
using System.Collections;

public class EngiCitizenshipController : MonoBehaviour {

    public EngiSkill0Controller skill0Controller;
    public EngineerController engineerController;
    public TeammatePanelController tpController;

    public void Clicked() {
        if (skill0Controller.IsSkillSelected()) {
            engineerController.HealCitizenship();
            skill0Controller.DeslectedSkill();
            skill0Controller.StartCoolDown();
            tpController.NoGlow();
        }
    }
}
