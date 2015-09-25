using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

	protected string role;
	protected int level;
	protected int health;
	protected Skill[] skills;

	void Awake(){
		role = "Striker";
		skills = new Skill[4];
		skills[0] = gameObject.GetComponent<StrikerSkill1>();
		skills[1] = gameObject.GetComponent<StrikerSkill2>();
		skills[2] = gameObject.GetComponent<StrikerSkill3>();
		skills[3] = gameObject.GetComponent<SupportSkill>();

  	}
	public Skill[] getSkills(){ return skills; }
	public Skill getSkill(int index) { 
		Debug.Log (index);
		return skills[index]; 
	}
	public int getLevel(){ return level; }
}
