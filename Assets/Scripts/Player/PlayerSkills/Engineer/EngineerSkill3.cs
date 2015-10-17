using UnityEngine;
using System.Collections;

public class EngineerSkill3 : Skill {

    void Start()
    {
        skillName = "healall";
        int level = gameObject.GetComponent<PlayerInfo>().getLevel();
        damage = Mathf.Pow(2, level) * 10 + 200;
        heal = 0;
        coolDown = 75;
    }
}
