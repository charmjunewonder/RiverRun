using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EngiTeammateController : MonoBehaviour {

    public EngiSkill0Controller skill0Controller;
    public CrystalProductionController crystalProductionController;

    private float maxCooldown;

    private EngineerController engineerController;

    private float cooldown;
    private bool showCoolDown;


    void Start() {
        cooldown = -1.0f;
        maxCooldown = 6.0f;
        showCoolDown = false;
        transform.GetChild(3).GetComponent<Text>().text = "";
        transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
    }

    void Update() {
        if (cooldown >= 0){
            cooldown -= Time.deltaTime;
            transform.GetChild(2).GetComponent<Image>().fillAmount = cooldown / maxCooldown;

            if (showCoolDown)
            {
                transform.GetChild(3).GetComponent<Text>().text = ((int)cooldown).ToString();

                if (cooldown < 0)
                {
                    transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
                    transform.GetChild(3).GetComponent<Text>().text = "";
                    cooldown = -1;
                }
            }

        }
    }

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

            transform.parent.GetComponent<TeammatePanelController>().NoGlow();

        } else if (crystalProductionController.isFinished()) {

            if (cooldown < 0) {
                engineerController.CmdAssignCrystal(num, crystalProductionController.GetCrystal());

                crystalProductionController.Revoke();

                transform.parent.GetComponent<TeammatePanelController>().DisableCrystalCoolDown();

                cooldown = maxCooldown;
            }
        }
    }

    public void setEngineerController(EngineerController ec) { engineerController = ec; }

    public void setShowCoolDown(bool f) { 
        
        showCoolDown = f;
        if (f) {
            transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else {
            transform.GetChild(3).GetComponent<Text>().text = "";
            transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    
    }
}
