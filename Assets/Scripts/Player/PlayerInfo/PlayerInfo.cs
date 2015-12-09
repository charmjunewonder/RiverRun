using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerInfo :  NetworkBehaviour{

	public PlayerRole role;
	protected int level;
    private float prevHealth;
    [SyncVar(hook = "OnChange")]
	public float health;

    public float max_health;
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
        prevHealth = health;
  	}
	public Skill[] getSkills(){ return skills; }
	public Skill getSkill(int index) { return skills[index]; }

    public float getHealth() { return health; }
    public void setHealth(float h) {
        health = h;

        if (health < 0)
            health = 0;
        if (health > max_health)
            health = max_health;


        if (isLocalPlayer) {
            int perc = health == 0 ? 0 : (int)(health / max_health * 10) + 1;

            healthController.setHealthPerc(health / max_health);

        }
    }

	public int getLevel(){ return level; }

    public void Damage(float damage) {
        health -= damage;

        if (health < 0)
            health = 0;
        if (health > max_health)
            health = max_health;

    }

    public float getMaxHealth() { return max_health; }

    private void OnChange(float f) {
        if (healthController == null) return;

        //Debug.Log("Health Hook " + f);
        prevHealth = health;

        health = f;

        if (health < prevHealth && health < 4)
        {
            AudioController.Singleton.PlayBloodLowSound();
        }

        int perc = health == 0 ? 0 : (int)(health / max_health * 10) + 1;

        healthController.setHealthPerc(health / max_health);
    }
}
