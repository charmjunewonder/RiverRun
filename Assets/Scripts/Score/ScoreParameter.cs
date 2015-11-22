

public class ScoreParameter
{

    public static int CurrentFullExp(int rank)
    {
        if (rank < 0) return 100;
        return (rank - 1) * 30 + 100;
    }

    public static int CalcuateScore(int skill1Counter, int skill2Counter)
    {
        if (skill1Counter < 0) skill1Counter = 0;
        if (skill2Counter < 0) skill2Counter = 0;

        return skill1Counter + skill2Counter * 15;
    }
}
