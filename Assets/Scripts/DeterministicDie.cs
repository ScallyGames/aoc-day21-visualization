public class DeterministicDie : IDie
{
    private int numberOfRolls;
    private int lastRoll = 0;

    public int GetNumberOfRolls()
    {
        return numberOfRolls;
    }

    public int[] GetRollResult()
    {
        numberOfRolls++;
        lastRoll++;
        lastRoll = ((lastRoll - 1) % 100) + 1;
        return new int[] { lastRoll };
    }
}