using UnityEngine;
using System.Collections;

public class StrikerSkill2 : Skill {

	void Start () {
		skillName = "stringhit";
		int level = gameObject.GetComponent<PlayerInfo>().getLevel();
		damage = level * level * 20;
		heal = 0;
		coolDown = 5;
	}
}
