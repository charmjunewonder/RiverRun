using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerInfo :  NetworkBehaviour{

	public PlayerRole role;
	protected int level;

    [SyncVar]
	protected float health;

    protected float max_health = 10.0f;
	protected Skill[] skills;

    public HealthController healthController;

    public void setHealthController(HealthController h) { healthController = h; }

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
        if (health > 10)
            health = 10;

        int perc = health == 0 ? 0 : (int)(health / max_health * 10) + 1;

        Debug.Log("perc " + health / max_health * 10);

        healthController.setHealth(perc);

        /*for (int i = 0; i < skills.Length; i++) {
            skills[i].damage *= (health / max_health);
            skills[i].heal *= (health / max_health);
        }*/
    }
}
