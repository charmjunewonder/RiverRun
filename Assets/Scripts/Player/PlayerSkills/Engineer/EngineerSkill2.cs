using UnityEngine;
using System.Collections;

public class EngineerSkill2 : Skill {

    void Start()
    {
        skillName = "produce crystal";
        int level = gameObject.GetComponent<PlayerInfo>().getLevel();
        damage = 0;
        heal = 0;
        coolDown = 5 / (level + 1);
    }

}
