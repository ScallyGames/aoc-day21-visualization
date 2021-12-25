using System;

public class DiracDie : IDie
{

    public int GetNumberOfRolls()
    {
        throw new Exception("What even is number of rolls with quantum mechanics");
    }

    public int[] GetRollResult()
    {
        return new int[] { 1, 2, 3 };
    }
}