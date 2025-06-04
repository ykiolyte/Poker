using System.Collections.Generic;
using Poker.Domain.Betting;

namespace Poker.Game.Betting
{
    /// <summary>Контейнер всех раундов ставок внутри одной раздачи.</summary>
    public sealed class BetHistory
    {
        public readonly List<BetRound> Rounds = new();
        public void AddRound(BetRound round) => Rounds.Add(round);
        public void Clear() => Rounds.Clear();
    }

    public sealed class BetRound
    {
        public readonly Street Street;
        public readonly List<PlayerBet> Actions;

        public BetRound(Street street, List<PlayerBet> actions)
        {
            Street  = street;
            Actions = actions;
        }
    }

    public enum Street { Preflop, Flop, Turn, River }
}
