using UnityEngine;
using System.Collections;

public abstract class Skill : MonoBehaviour{

	public GameObject prefab;
	protected string skillName;
	protected double damage;
	protected double heal;
	protected float coolDown;

	public float getCoolDown(){ return coolDown;}

}
