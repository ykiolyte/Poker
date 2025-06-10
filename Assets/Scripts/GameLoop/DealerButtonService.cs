using System.Collections.Generic;
using UnityEngine;

namespace Poker.GameLoop
{
    /// <summary>
    /// Управляет позицией дилера. Даёт индексы SB/BB и порядок игроков.
    /// </summary>
    public sealed class DealerButtonService : MonoBehaviour
    {
        private IList<PokerPlayerController> players;
        private int dealerIndex = -1;

        public int DealerIndex     => dealerIndex;
        public int SmallBlindIndex => players.Count == 0 ? -1 : (dealerIndex + 1) % players.Count;
        public int BigBlindIndex   => players.Count == 0 ? -1 : (dealerIndex + 2) % players.Count;

        public void Initialize(IList<PokerPlayerController> list)
        {
            players = list;
            dealerIndex = players.Count - 1;
        }

        public int NextDealer()
        {
            if (players == null || players.Count == 0) return dealerIndex;
            dealerIndex = (dealerIndex + 1) % players.Count;
            return dealerIndex;
        }

        public IEnumerable<PokerPlayerController> DealingOrder()
        {
            if (players == null) yield break;

            int start = SmallBlindIndex;
            for (int i = 0; i < players.Count; i++)
                yield return players[(start + i) % players.Count];
        }
    }
}
