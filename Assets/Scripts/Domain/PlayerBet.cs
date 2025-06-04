namespace Poker.Domain.Betting
{
    /// <summary>Связка игрок–ставка для журналирования раунда.</summary>
    public readonly struct PlayerBet
    {
        public int PlayerId { get; }
        public Bet Bet { get; }

        public PlayerBet(int playerId, Bet bet)
        {
            PlayerId = playerId;
            Bet = bet;
        }

        public override string ToString() => $"Player {PlayerId}: {Bet}";
    }
}
