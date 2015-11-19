using UnityEngine;
using System.Collections;

public class StrikerSkill1 : Skill {

	void Start(){
		skillName = "hit";
		int level = gameObject.GetComponent<PlayerInfo>().getLevel();
		damage = 5;
		heal = 0;
		coolDown = 0.5f;
	}
}
