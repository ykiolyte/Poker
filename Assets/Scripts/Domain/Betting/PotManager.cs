using System.Collections.Generic;
using System.Linq;
using Poker.GameLoop;
using UnityEngine;

namespace Poker.Domain.Betting
{
    /// <summary>
    /// Собирает основной банк и сайд-поты по мере поступления ставок.
    /// Работает поверх существующего <see cref="Pot"/> —
    /// не ломает старый код, который считал общий Total.
    /// </summary>
    public sealed class PotManager
    {
        private readonly Dictionary<int, int> _contrib = new();   // seat → вклад
        private readonly Pot _mainPot = new();                    // прежний банк

        /* ---------- API для ставок ---------- */

        public void AddBet(PokerPlayerModel p, int amount)
        {
            _mainPot.AddBet(p, amount);                   // старый счётчик Total
            _contrib[p.Id] = _contrib.GetValueOrDefault(p.Id) + amount;
        }

        /// <summary>Копия словаря вкладов для сайд-потов.</summary>
        public Dictionary<int, int> Contributions => new(_contrib);

        public int Total => _mainPot.Total;

        public void Reset()
        {
            _contrib.Clear();
            _mainPot.Reset();
        }

        /* ---------- Распределение выигрышей ---------- */

        public Dictionary<int, int> Distribute(Dictionary<int, HandResult> hands)
        {
            var payout  = new Dictionary<int, int>();
            var working = new Dictionary<int, int>(_contrib); // mutable copy

            while (working.Count > 0)
            {
                int minBet = int.MaxValue;
                foreach (var v in working.Values) if (v < minBet) minBet = v;

                // Чей вклад ≥ minBet — претенденты на текущий банк
                List<int> contenders = new();
                foreach (var kv in working)
                    if (kv.Value >= minBet) contenders.Add(kv.Key);

                int potAmount = minBet * working.Count;
                foreach (int key in working.Keys.ToArray())
                    working[key] -= minBet;
                foreach (var k in working.Where(k => k.Value == 0).Select(k => k.Key).ToArray())
                    working.Remove(k);

                // Выбираем лучшую руку среди претендентов
                var best = contenders[0];
                foreach (int c in contenders)
                    if (hands[c].Score.CompareTo(hands[best].Score) > 0) best = c;

                // Делим банк между равными руками
                var winners = contenders.FindAll(c =>
                    hands[c].Score.CompareTo(hands[best].Score) == 0);

                int share = potAmount / winners.Count;
                foreach (int w in winners)
                    payout[w] = payout.GetValueOrDefault(w) + share;
            }

            return payout;
        }
    }
}
