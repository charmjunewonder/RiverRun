

public class ScoreParameter
{

    public static int CurrentFullExp(int rank)
    {
        if (rank < 0) return 100;
        return (rank - 1) * 30 + 100;
    }

    public static int CalcuateScore(int skill1Counter, int skill2Counter, int supportCounter)
    {
        if (skill1Counter < 0) skill1Counter = 0;
        if (skill2Counter < 0) skill2Counter = 0;
        if (supportCounter < 0) supportCounter = 0;

        return skill1Counter + skill2Counter * 15 + supportCounter*3;
    }

    public static int CalcuateStar(int score)
    {
        if (score < 0) score = 0;
        int stars = score / 50;
        if (stars < 0) stars = 0;
        if (stars > 4) stars = 4;
        return stars;
    }
}
