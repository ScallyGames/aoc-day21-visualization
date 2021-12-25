using System;

public struct PlayerState : IEquatable<PlayerState>
{
    public int CurrentPosition;
    public int CurrentScore;
    
    public PlayerState(int CurrentPosition, int CurrentScore)
    {
        this.CurrentPosition = CurrentPosition;
        this.CurrentScore = CurrentScore;
    }
    public bool HasWon => CurrentScore >= GameSettings.WinningScore;

    public bool Equals(PlayerState other)
    {
        return CurrentPosition == other.CurrentPosition && CurrentScore == other.CurrentScore;
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerState other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (CurrentPosition * 397) ^ CurrentScore;
        }
    }
}