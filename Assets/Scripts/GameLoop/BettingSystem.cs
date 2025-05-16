using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.GameLoop
{
    /// <summary>
    /// MVP-реализация ставок: SB & BB, затем все коллируют до BB.
    /// Рейзы, тайм-ауты и side-pot появятся на следующем шаге.
    /// </summary>
    public sealed class BettingSystem : MonoBehaviour
    {
        [SerializeField] private int bigBlind   = 20;
        [SerializeField] private int smallBlind = 10;

        public int Pot { get; private set; }

        public IEnumerator ExecuteBettingRound(List<PokerPlayerController> players)
        {
            Pot = 0;

            // 1) small / big blind (предполагаем, что players[0] — SB, players[1] — BB)
            if (players.Count >= 2)
            {
                Charge(players[0], smallBlind);
                Charge(players[1], bigBlind);
            }

            int toCall = bigBlind;

            // 2) оставшиеся коллируют
            foreach (var p in players)
            {
                if (p.Model.IsFolded) continue;

                int diff = toCall - p.Model.TotalBet;
                if (diff > 0) Charge(p, diff);
            }

            yield return null;                       // микропауза для реализма
            Debug.Log($"[Bet] Round finished, pot = {Pot}");
        }

        #region helpers ------------------------------------------------------

        private void Charge(PokerPlayerController p, int amount)
        {
            amount = Mathf.Min(amount, p.Model.Stack);
            p.Model.AddChips(-amount);          // уменьшили стек
            p.Model.TotalBet += amount;         // учли ставку
            Pot              += amount;

            Debug.Log($"[Bet] P#{p.Model.Id} → bet {amount}, stack now {p.Model.Stack}");
        }

        #endregion
    }
}
