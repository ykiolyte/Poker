namespace Poker.Game.Betting
{
    public enum BettingActionType
    {
        None,
        Check,
        Call,
        Raise,
        AllIn,
        Fold
    }

    public class BettingAction
    {
        public BettingActionType ActionType { get; }
        public int Amount { get; }

        public BettingAction(BettingActionType type, int amount = 0)
        {
            ActionType = type;
            Amount = amount;
        }
    }
}
