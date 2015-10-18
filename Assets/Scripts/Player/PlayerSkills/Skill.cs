using UnityEngine;
using System.Collections;

public abstract class Skill : MonoBehaviour{

	public GameObject prefab;
	public string skillName;
    public float damage;
    public float heal;
    public float coolDown;

	public float getCoolDown(){ return coolDown;}

}
