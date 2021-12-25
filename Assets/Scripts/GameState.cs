using System;

public struct GameState : IEquatable<GameState>
{
    public PlayerState Player0State;
    public PlayerState Player1State;
    public int PlayerOnTurn;
    public GameState(PlayerState Player0State, PlayerState Player1State, int PlayerOnTurn)
    {
        this.Player0State = Player0State;
        this.Player1State = Player1State;
        this.PlayerOnTurn = PlayerOnTurn;
    }
    
    public bool IsCompleted => Player0State.HasWon || Player1State.HasWon;
    public int MaxScore => Math.Max(Player0State.CurrentScore, Player1State.CurrentScore);
    public int MinScore => Math.Min(Player0State.CurrentScore, Player1State.CurrentScore);
    public bool IsPlayer0Winning => Player0State.CurrentScore > Player1State.CurrentScore;

    public bool Equals(GameState other)
    {
        return Player0State.Equals(other.Player0State) && Player1State.Equals(other.Player1State) && PlayerOnTurn == other.PlayerOnTurn;
    }

    public override bool Equals(object obj)
    {
        return obj is GameState other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Player0State.GetHashCode();
            hashCode = (hashCode * 397) ^ Player1State.GetHashCode();
            hashCode = (hashCode * 397) ^ PlayerOnTurn;
            return hashCode;
        }
    }
}