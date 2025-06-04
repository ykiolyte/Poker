using System.Collections.Generic;

namespace Poker.Domain.Betting
{
    /// <summary>Собирает ставки до перехода на следующую street.</summary>
    public sealed class BettingRound
    {
        private readonly List<PlayerBet> _bets = new();
        public IReadOnlyList<PlayerBet> Bets => _bets;

        public Pot Pot { get; } = new Pot();

        public void RecordBet(PokerPlayerModel player, Bet bet)
        {
            _bets.Add(new PlayerBet(player.Id, bet));

            if (bet.Action is BetAction.Raise or BetAction.Call)
                Pot.AddBet(player, bet.Amount);
        }

        public void Reset()
        {
            _bets.Clear();
            Pot.Reset();
        }
    }
}
