using UnityEngine;
using System.Collections;

public class EngiSkillSwitch : MonoBehaviour {

    private int skillSelected = 0;

    public void SetSkill(int index) {
        if (index == skillSelected) {
            if (index == 0)
            {
                transform.parent.GetChild(5).GetComponent<TeammatePanelController>().SetGlow();
            }
            else
            {
                transform.GetChild(0).GetComponent<EngiSkill0Controller>().DeslectedSkill();
                transform.parent.GetChild(5).GetComponent<TeammatePanelController>().NoGlow();
                transform.parent.GetChild(5).GetComponent<TeammatePanelController>().EnableCrystalCoolDown();
            }
            return;
        }
            
        skillSelected = index;
        if (index == 0) {
            transform.GetChild(1).GetComponent<EngiSkill1Controller>().Revoke();
            transform.parent.GetChild(5).GetComponent<TeammatePanelController>().SetGlow();
        } else if (index == 1) {
            transform.GetChild(0).GetComponent<EngiSkill0Controller>().DeslectedSkill();
            transform.parent.GetChild(5).GetComponent<TeammatePanelController>().NoGlow();
            transform.parent.GetChild(5).GetComponent<TeammatePanelController>().EnableCrystalCoolDown();
        }
    }
}
