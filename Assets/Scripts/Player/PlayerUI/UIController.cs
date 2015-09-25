using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public Image coolDownImage;
	public Text coolDownText;

	private bool coolDownStarted = false;
	private float coolDownTime;
	private float coolDownTimer;

	private Skill skill;

	public PlayerController playerController;

	public void selectSkill(int index){
		playerController.CmdSetSkillIndex (index);
	}

	public void StartCoolDown(){
		coolDownTime = skill.getCoolDown ();
		coolDownTimer = coolDownTime;
		coolDownText.text = (int)coolDownTimer + 1 + "";
		coolDownImage.fillAmount = 1;
		coolDownStarted = true;
	}

	private void coolingDown(){
		coolDownTimer -= Time.deltaTime;
		if(coolDownTimer <= 0.0f){
			coolDownStarted = false;
			coolDownImage.fillAmount = 0;
			coolDownText.text = "";
			return;
		}
		coolDownImage.fillAmount = (coolDownTimer / coolDownTime);
		coolDownText.text = (int)coolDownTimer + 1 + "";
	}



	void update(){
		if (coolDownStarted) {
			coolingDown();
		}
	}

	public void setSkill(Skill s){ skill = s; }
	public void setPlayerController(PlayerController p){ playerController = p; }
}
