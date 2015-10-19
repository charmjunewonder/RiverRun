using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

	public PlayerRole role;
	public int level;
	public float health;

    protected float max_health;
	protected Skill[] skills;

	void Awake(){
        if (role == PlayerRole.Engineer) {
            skills = new Skill[3];
            skills[0] = gameObject.GetComponent<EngineerSkill1>();
            skills[1] = gameObject.GetComponent<EngineerSkill2>();
            skills[2] = gameObject.GetComponent<EngineerSkill3>();
        } else if (role == PlayerRole.Defender)
        {
            skills = new Skill[2];
            skills[0] = gameObject.GetComponent<StrikerSkill1>();
            skills[1] = gameObject.GetComponent<StrikerSkill2>();
        } else {
            skills = new Skill[2];
            skills[0] = gameObject.GetComponent<StrikerSkill1>();
            skills[1] = gameObject.GetComponent<StrikerSkill2>();
        }
        level = 1;
        health = 10;
  	}
	public Skill[] getSkills(){ return skills; }
	public Skill getSkill(int index) { 
		return skills[index]; 
	}
	public int getLevel(){ return level; }

    public void Damage(float damage) {
        health -= damage;

        if (health < 0)
            health = 0;

        for (int i = 0; i < skills.Length; i++) {
            skills[i].damage *= (health / max_health);
            skills[i].heal *= (health / max_health);
        }
    }
}
