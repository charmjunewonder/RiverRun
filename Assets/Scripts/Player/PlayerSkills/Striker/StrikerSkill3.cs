using UnityEngine;
using System.Collections;

public class StrikerSkill3 : Skill {

	void Start () {
		skillName = "aoe";
		int level = gameObject.GetComponent<PlayerInfo>().getLevel();
		damage = Mathf.Pow (2, level) * 10 + 200;
		heal = 0;
		coolDown = 1;
	}

	void Update () {
	
	}
}
