using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EngiTeammateController : MonoBehaviour {

    public EngiSkill0Controller skill0Controller;
    public CrystalProductionController crystalProductionController;

    private EngineerController engineerController;


    public void Highlight() {
        if (skill0Controller.IsSkillSelected() || crystalProductionController.isFinished())
            transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void NoHighlight() {
        transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);        
    }

    public void AssignToTeammate(int num) {
        if (skill0Controller.IsSkillSelected()) {

            engineerController.CmdHealTeammate(num);

            skill0Controller.DeslectedSkill();

            skill0Controller.StartCoolDown();

        } else if (crystalProductionController.isFinished()) {
            engineerController.CmdAssignCrystal(num, crystalProductionController.GetCrystal());
            
            crystalProductionController.Revoke();
        }
    }

    public void setEngineerController(EngineerController ec) { engineerController = ec; }

}
