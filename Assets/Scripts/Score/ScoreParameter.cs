

public class ScoreParameter
{
    public static float Personal_Health_Penalty_Persent = 0.01f;
    public static float Citizen_Health_Penalty_Persent = 0.02f;
    public static int Boss_Score = 120;

    public static int Stricker_Skill1_Score = 1;
    public static int Stricker_Util_Score = 40;

    public static int Defender_Skill1_Score = 1;
    public static int Defender_Util_Score = 40;

    public static int Engineer_Skill1_Score = 3;
    public static int Engineer_Skill2_Score = 5;

    public static int Support_Score = 5;

    public static int CurrentFullExp(int rank)
    {
        if (rank < 0) return 100;
        return rank * rank * 15 + 100 * rank;
    }

    public static int CalcuateScore(int skill1Counter, int skill2Counter, int supportCounter, int i)
    {
        if (skill1Counter < 0) skill1Counter = 0;
        if (skill2Counter < 0) skill2Counter = 0;
        if (supportCounter < 0) supportCounter = 0;
        return skill1Counter + skill2Counter * 15 + supportCounter*3;
    }

    public static int CalcuateStar(int score)
    {
        if (score < 0) score = 0;
        int stars = score / 70;
        if (stars < 0) stars = 0;
        if (stars > 4) stars = 4;
        return stars;
    }
}
