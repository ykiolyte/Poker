using System;
using System.Collections.Generic;
using System.Linq;
using Poker.Gameplay.Cards;

namespace Poker.GameLoop
{
    /// <summary>
    /// Выбирает лучшую руку из 7 карт и возвращает только значимые карты
    /// (например, «пара + кикер» вместо всей пятёрки).
    /// </summary>
    public static class HandEvaluator
    {
        public static HandResult EvaluateBestHand(List<CardDataSO> hole, List<CardDataSO> board)
        {
            if (hole == null || board == null) throw new ArgumentNullException();

            var pool = hole.Concat(board).Where(c => c != null).ToArray();
            if (pool.Length < 5)
                throw new ArgumentException("Нужно минимум 5 карт в пуле — дождитесь ривера.");

            HandScore         bestScore  = null;
            List<CardDataSO>  bestCombo  = null;
            var idx = new[] { 0, 1, 2, 3, 4 };

            // перебираем все C(n,5) комбинации
            while (true)
            {
                var combo = idx.Select(i => pool[i]).ToList();
                var score = EvaluateFive(combo);

                if (bestScore == null || score.CompareTo(bestScore) > 0)
                {
                    bestScore = score;
                    bestCombo = combo;
                }

                int t = 4;
                while (t >= 0 && idx[t] == t + pool.Length - 5) t--;
                if (t < 0) break;
                idx[t]++;
                for (int j = t + 1; j < 5; j++)
                    idx[j] = idx[j - 1] + 1;
            }

            return new HandResult(bestScore, Reduce(bestScore, bestCombo));
        }

        #region Five-card evaluation -----------------------------------------

        private static HandScore EvaluateFive(List<CardDataSO> cards)
        {
            var ranks = cards.Select(c => (int)c.rank).OrderByDescending(x => x).ToArray();
            var suits = cards.Select(c => c.suit).ToArray();

            bool isFlush    = suits.All(s => s == suits[0]);
            bool isStraight = IsStraight(ranks, out int highStraight);

            var groups = ranks
                .GroupBy(r => r)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .ToArray();

            if (isStraight && isFlush)
                return new HandScore(highStraight == 14 ? HandRank.RoyalFlush : HandRank.StraightFlush,
                                     new[] { highStraight });

            if (groups[0].Count() == 4)
                return new HandScore(HandRank.FourOfAKind, Flatten(groups));

            if (groups[0].Count() == 3 && groups[1].Count() == 2)
                return new HandScore(HandRank.FullHouse, Flatten(groups));

            if (isFlush)
                return new HandScore(HandRank.Flush, ranks);

            if (isStraight)
                return new HandScore(HandRank.Straight, new[] { highStraight });

            if (groups[0].Count() == 3)
                return new HandScore(HandRank.ThreeOfAKind, Flatten(groups));

            if (groups[0].Count() == 2 && groups[1].Count() == 2)
                return new HandScore(HandRank.TwoPair, Flatten(groups));

            if (groups[0].Count() == 2)
                return new HandScore(HandRank.OnePair, Flatten(groups));

            return new HandScore(HandRank.HighCard, ranks);
        }

        private static bool IsStraight(int[] ranks, out int high)
        {
            high = 0;
            var distinct = ranks.Distinct().ToList();
            if (distinct.Contains(14)) distinct.Add(1);        // Ace-low

            distinct.Sort((a, b) => b.CompareTo(a));

            for (int i = 0; i <= distinct.Count - 5; i++)
            {
                bool ok = true;
                for (int j = 0; j < 4; j++)
                    if (distinct[i + j] - 1 != distinct[i + j + 1])
                    { ok = false; break; }

                if (ok) { high = distinct[i]; return true; }
            }
            return false;
        }

        #endregion

        #region Helpers ------------------------------------------------------

        private static int[] Flatten(IEnumerable<IGrouping<int, int>> g) =>
            g.SelectMany(x => Enumerable.Repeat(x.Key, x.Count())).ToArray();

        /// <summary>
        /// Оставляем только «суть» комбинации для красивого вывода.
        /// </summary>
        private static List<CardDataSO> Reduce(HandScore score, List<CardDataSO> combo)
        {
            var ordered = combo.OrderByDescending(c => c.rank).ToList();

            switch (score.Rank)
            {
                case HandRank.HighCard:
                    return ordered.Take(1).ToList();

                case HandRank.OnePair:
                    {
                        var pairRank = ordered.GroupBy(c => c.rank).First(g => g.Count() == 2).Key;
                        var pair   = ordered.Where(c => c.rank == pairRank).ToList();
                        var kicker = ordered.First(c => c.rank != pairRank);
                        pair.Add(kicker);
                        return pair;
                    }

                case HandRank.TwoPair:
                    {
                        var pairs  = ordered.GroupBy(c => c.rank).Where(g => g.Count() == 2)
                                           .Take(2).SelectMany(g => g).ToList();
                        var kicker = ordered.First(c => !pairs.Contains(c));
                        pairs.Add(kicker);
                        return pairs;
                    }

                case HandRank.ThreeOfAKind:
                    {
                        var tripleRank = ordered.GroupBy(c => c.rank).First(g => g.Count() == 3).Key;
                        var triple = ordered.Where(c => c.rank == tripleRank).ToList();
                        triple.Add(ordered.First(c => c.rank != tripleRank));  // старший кикер
                        return triple;
                    }

                default:        // Straight, Flush, FullHouse, Four-/Straight-Flush
                    return combo; // для этих рук нужна вся пятёрка
            }
        }

        #endregion
    }
}
