using UnityEngine;
using System.Collections;

public class EngiSkill1Controller : SkillController {

    public CrystalProductionController crystalProductionPanel; 

    public void SkillPressed() {
       transform.parent.GetComponent<EngiSkillSwitch>().SetSkill(1);
       crystalProductionPanel.TriggerAnimation(true);
       
    }

    public void SkillReleased() {
        if (!crystalProductionPanel.isFinished()) {
            crystalProductionPanel.Revoke();
        }
    }

    public void Revoke() {
        crystalProductionPanel.Revoke();
    }
}
