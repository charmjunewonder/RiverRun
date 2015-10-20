using UnityEngine;
using System.Collections;

public class EngineerSkill1 : Skill {
    void Start() {
        skillName = "heal";
        int level = gameObject.GetComponent<PlayerInfo>().getLevel();
        damage = 0;
        heal = level * 2;
        coolDown = 3;
    }
}
