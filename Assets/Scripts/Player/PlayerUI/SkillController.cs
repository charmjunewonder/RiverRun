using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillController : MonoBehaviour {

	public Image coolDownImage;
	public Text coolDownText;

	private bool coolDownStarted = false;
	private float coolDownTime;
	private float coolDownTimer;

	private Skill skill;

	public PlayerController playerController;

	public void selectSkill(int index){
		if (coolDownStarted) return;
        if(index <= 1)
		    playerController.CmdSetSkillIndex (index);
        else if (index == 2) { 
            
        }
        
	}

	public void StartCoolDown(){
		coolDownTime = skill.getCoolDown ();
		coolDownTimer = coolDownTime;
		coolDownText.text = (int)coolDownTimer + 1 + "";
		coolDownImage.fillAmount = 1;
		coolDownStarted = coolDownTime == 0 ? false : true;
	}

	private void coolingDown(){
		coolDownTimer -= Time.deltaTime;
        Debug.Log (coolDownTimer);
		if(coolDownTimer <= 0.0f){
			coolDownStarted = false;
			coolDownImage.fillAmount = 0;
			coolDownText.text = "";
			return;
		}
		coolDownImage.fillAmount = (coolDownTimer / coolDownTime);
		coolDownText.text = (int)coolDownTimer + 1 + "";
	}



	void Update(){
		if (coolDownStarted) {
			coolingDown();
		}
	}

	public void setSkill(Skill s){ skill = s; }
	public void setPlayerController(PlayerController p){ playerController = p; }
	public bool getCoolDownStatus(){ return coolDownStarted; }
}
