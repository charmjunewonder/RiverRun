using UnityEngine;
using System.Collections;

public class StrikerSkill1 : Skill {

	void Start(){
		skillName = "hit";
		int level = gameObject.GetComponent<PlayerInfo>().getLevel();
		damage = level * 2;
		heal = 0;
		coolDown = 1;
	}
}
