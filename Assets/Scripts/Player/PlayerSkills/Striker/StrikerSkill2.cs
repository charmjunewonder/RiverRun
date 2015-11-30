using UnityEngine;
using System.Collections;

public class StrikerSkill2 : Skill {
    void Start()
    {
        skillName = "aoe";
        int level = gameObject.GetComponent<PlayerInfo>().getLevel();
        damage = 20;
        heal = 0;
        coolDown = 60;
    }
}
