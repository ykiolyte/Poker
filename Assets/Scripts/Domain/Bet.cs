namespace Poker.Domain.Betting
{
    /// <summary>Value Object: единичное решение игрока в ставочном раунде.</summary>
    public readonly struct Bet
    {
        public BetAction Action { get; }
        public int Amount { get; }

        public Bet(BetAction action, int amount = 0)
        {
            Action = action;
            Amount = amount;

            // Валидация
            switch (Action)
            {
                case BetAction.Raise:
                case BetAction.Call:
                    if (amount <= 0) throw new System.ArgumentOutOfRangeException(nameof(amount),
                        "Для Raise/Call сумма должна быть положительной.");
                    break;
                case BetAction.Check:
                case BetAction.Fold:
                    if (amount != 0) throw new System.ArgumentOutOfRangeException(nameof(amount),
                        "Для Check/Fold сумма должна быть равна нулю.");
                    break;
            }
        }

        public override string ToString() =>
            Amount > 0 ? $"{Action} {Amount}" : Action.ToString();
    }

    public enum BetAction
    {
        Check,
        Call,
        Raise,
        Fold
    }
}
