using UnityEngine;
using System.Collections;

/* class Enemy Parameter
 * First use .getEnemy(int rank) to update&get updated data <EnemyParameter>
 * .enemyWave --- wave number of enemies
 * .enemyNumbers[wave] --- enemies numbers of each wave 
 * .enemyDatas[wave][enemyNumber] --- data of each enemy as struct EnemyData
 * struct EnemyData 
 *        --- .maxHp initial Hp <int>
 *        --- .attackPt attack point <int>
 *        --- .attackTime time between two attacks <float>
 */

public struct EnemyData //enemy data is associated with difficulty factor
{
    //string enemyType
    public int maxHp;
    public int attackPt;
    public float attackTime;
    public EnemyData(int maxHp, int attackPt, float attackTime)
    {
        this.maxHp = maxHp;
        this.attackPt = attackPt;
        this.attackTime = attackTime;
    }
}

public class EnemyParameter : MonoBehaviour {
    private int difficultyFactor = 1;//need to be calculated
    private int enemyBasicHp = 25;
    private int enemyBasicAp = 2;
    private float enemyBasicAttackTime = 8;
    public int enemyWave = 2; //from difficulty factor
    private int maxNumberPerWave = 20;
    public int[] enemyNumbers = new int[2];//enemy numbers ineach wave

    

    public EnemyData[][] enemyDatas;//2d array to store enemy data [wave][order] 

    public AnimationCurve wave_number;
    //public AnimationCurve wave_frenquency;

    private void updateBasicData(int diffFactor)
    {
        difficultyFactor = diffFactor;
        enemyBasicHp = 25 + 4 * difficultyFactor;
        enemyBasicAp = 2 + (difficultyFactor / 5);//temp
        enemyBasicAttackTime = Mathf.Clamp(8.0f - difficultyFactor * 0.4f, 0.5f, 8.0f);//change the enemy attack frenquency
        maxNumberPerWave = 20 + 3 * difficultyFactor; // maximum 50 
        enemyWave = 3 + ((difficultyFactor+2) / 4); //every 4 rank add a wave
    }

    private void generateEnemyData()
    {
        //use basic data to generate the enemy datas
        System.Array.Resize<int>(ref enemyNumbers, enemyWave);
        for (int i = 0; i < enemyWave; i++)
        {
            //factor from the animation curve
            float factor = wave_number.Evaluate((float)i / enemyWave);
            float enemyNumber = factor * maxNumberPerWave;
            // random ajust use 20 persent
            enemyNumbers[i] = (int)(enemyNumber*(1+(Random.value-0.50f)*0.20f));
        } 
        //create a jaggedArray for enemydatas and initialize
        EnemyData[][] enemyCollection = new EnemyData[enemyWave][];
        for (int i = 0; i < enemyWave; i++)
        {//initial the jagged data
            enemyCollection[i] = new EnemyData[enemyNumbers[i]];
        }

        for (int i = 0; i < enemyWave; i++)
        {
            for (int j = 0; j < enemyNumbers[i]; j++)
            {
                //create the data
                 int maxHp =(int) (enemyBasicHp*(1+(Random.value-0.50f)*0.1f));
                 int attackPt = (int)(enemyBasicAp* (int)(1+(Random.value-0.50f)*0.1f));
                 float attackTime = enemyBasicAttackTime*(1.0f+(Random.value-0.50f)*0.1f);
                //put enemydata into datas
                 enemyCollection[i][j] = new EnemyData(maxHp, attackPt, attackTime);
            }
        }
        this.enemyDatas = enemyCollection;//update the enemy collections
    }


    public EnemyParameter getEnemys(int diffFactor)
    {
        //difficultyFactor = diffFactor;
        updateBasicData(diffFactor);
        generateEnemyData();
        return this; 
    }

    public void displayEnemyData()
    { //should addin null check
        int sumEnemy = 0;
        for (int i = 0; i < enemyWave; i++)
        {
            for (int j = 0; j < enemyNumbers[i]; j++)
            {
                Debug.Log("Hp"+enemyDatas[i][j].maxHp+
                    "Ap"+enemyDatas[i][j].attackPt+
                    "At"+enemyDatas[i][j].attackTime);
                sumEnemy++; 
            }
        }
        Debug.Log("SumEnemies" + sumEnemy);
    }
    
}
