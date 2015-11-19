using UnityEngine;
using System.Collections;
using System.Reflection;

/* class PlayerParameter
 * First use .getPlayer(Role role,int rank) to update and get updated class object
 * .maxHp --- player initial hp
 * .coolingDown_1 --- skill 1 coolingdown time
 * .coolingDown_2 --- skill 2 coolingdown time
 * .attackPt ---- striker attack point
 * .ultiPt ---- striker ultimate skill attack point
 * .healPt ---- engineer heal point
 * .playerCoolingDown --- engineer assign crystal for each player coolingdown time
 * .ultiTime --- defender ultimate skill freze enemy time
 * .shieldTime --- defender skill 1 sheild time
 * .shieldHp --- defender skill 1 defend enemy attack number
 */

public class PlayerParameter:MonoBehaviour{

    /*Basic parameter*/
    private PlayerRole role = PlayerRole.Striker;//default role
    private int rank = 0;//range 0-15
    public int exp = 0;// need or not
    public int maxHp = 80;//health 80-155
    public float coolingDown_1 = 0.5f;//0.5 - 0.2 
    public float coolingDown_2 = 60.0f;//depend on role ultimate 60-30
    
    /*Striker*/
    public int attackPt = 5;//range 5-20 ---- enemy minions hp 20-30
    public int ultiPt = 10;//twice of attackPt of Striker
    
    /*Engineer*/
    public int healPt = 10;//range 10-25
    
    /*Defender*/
    public float ultiTime = 10.0f; //from 10 - 30 of Defender

    /* For modified defender and engineer some added parameters*/
    public float sheildTime = 3.0f;
    public int sheildHp = 2;

    public float playerCoolingDown = 4.0f; //twice as the produce crystal

    public PlayerParameter() { }
    public PlayerParameter(PlayerRole inputRole, int inputRank)
    {
        playerUpdate(inputRole, inputRank);
    }

    public PlayerParameter getPlayer(PlayerRole inputRole, int inputRank)
    {
        playerUpdate(inputRole, inputRank);//update the player data according to the input
        return this;

    }

    private void playerUpdate(PlayerRole inputRole, int inputRank)
    {
        role = inputRole;
        rank = inputRank;
        updateHp();//update the maxHp
        switch (role)
        {
            case PlayerRole.Striker:
                {
                    attackPt = 5 + rank; //striker attack
                    coolingDown_1 = 0.5f - (rank / 5)*0.1f; //coolingDown 1
                    coolingDown_2 = 60.0f - rank * 2.0f;//coolingDown 2
                    ultiPt = attackPt * 4;//ultimate skill attack
                    break;
                }
            case PlayerRole.Defender:
                {
                    coolingDown_1 = 0.5f - (rank / 5) * 0.1f; //coolingDown 1
                    coolingDown_2 = 60.0f - rank * 2.0f;//coolingDown 2
                    ultiTime = 10.0f + rank * 1.2f; // defender froze enemy time
                    sheildTime = 5.0f + rank * 0.2f;//defender shield time
                    sheildHp = 5 + (rank / 4) * 1;//defender shield defend attack number
                    break;
                }
            case PlayerRole.Engineer:
                {
                    healPt = 10 + rank;//engineer heal point
                    coolingDown_1 = 2.0f - (rank / 5) * 0.5f;//engineer heal coolingdown time
                    coolingDown_2 = 2.0f - (rank / 5) * 0.3f;//crystal produce time 
                    playerCoolingDown = coolingDown_2 * 2.0f;//player cooling down for assign crystals
                    break;
                }

            default:
                break;
        }

    }

    private void updateHp()
    {
        maxHp = 80 + rank * 5;
    }


    public void displayPlayerData()
    {
        string info = " ";
        const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public |
             BindingFlags.Instance | BindingFlags.Static;
        FieldInfo[] fields = this.GetType().GetFields(flags);
        foreach (FieldInfo fieldInfo in fields)
        {
            info += (fieldInfo.Name + ":" + fieldInfo.GetValue(this)+"\n");
            //Debug.Log(fieldInfo.Name+":"+fieldInfo.GetValue(this));
        }
        
        Debug.Log(info);
    }
	
}
