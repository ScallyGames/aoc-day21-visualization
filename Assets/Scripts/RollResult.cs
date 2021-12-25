public struct RollResult
{
    public int Roll;
    public byte NumberOfTimesRolled;
    
    public RollResult(int Roll, byte NumberOfTimesRolled)
    {
        this.Roll = Roll;
        this.NumberOfTimesRolled = NumberOfTimesRolled;
    }
}