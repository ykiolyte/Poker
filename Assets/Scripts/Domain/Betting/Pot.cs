using System.Collections.Generic;

namespace Poker.Domain.Betting
{
    /// <summary>Агрегат «банк» с учётом индивидуальных вкладов.</summary>
    public sealed class Pot
    {
        private readonly Dictionary<int,int> _contributions = new();
        public int Total { get; private set; }

        public void AddBet(PokerPlayerModel player, int amount)
        {
            if (player == null) throw new System.ArgumentNullException(nameof(player));
            if (!player.TryBet(amount))
                throw new System.InvalidOperationException(
                    $"Player {player.Id} cannot bet {amount} (stack {player.Stack}).");

            Total += amount;
            _contributions[player.Id] = GetContribution(player.Id) + amount;
        }

        public int GetContribution(int playerId) =>
            _contributions.TryGetValue(playerId, out var v) ? v : 0;

        public void Reset()
        {
            _contributions.Clear();
            Total = 0;
        }
    }
}
