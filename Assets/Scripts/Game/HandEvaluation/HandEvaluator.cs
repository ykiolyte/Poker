using System.Collections.Generic;
using System.Linq;

public static class HandEvaluator
{
    public static HandValue EvaluateHand(List<CardDataSO> holeCards, List<CardDataSO> communityCards)
    {
        var allCards = holeCards.Concat(communityCards).ToList();
        var groupedByRank = allCards.GroupBy(c => c.rank).ToList();
        var groupedBySuit = allCards.GroupBy(c => c.suit).ToList();

        var hand = new HandValue();

        // Проверка на флеш
        var flushSuitGroup = groupedBySuit.FirstOrDefault(g => g.Count() >= 5);
        bool isFlush = flushSuitGroup != null;
        var flushCards = isFlush ? flushSuitGroup.OrderByDescending(c => c.rank).ToList() : null;

        // Проверка на стрейт
        var distinctRanks = allCards.Select(c => (int)c.rank).Distinct().OrderByDescending(r => r).ToList();
        if (distinctRanks.Contains(14)) distinctRanks.Add(1); // A low

        int straightHigh = -1;
        for (int i = 0; i <= distinctRanks.Count - 5; i++)
        {
            if (distinctRanks[i] - 4 == distinctRanks[i + 4])
            {
                straightHigh = distinctRanks[i];
                break;
            }
        }

        bool isStraight = straightHigh != -1;

        // Стрит-флеш / Роял флеш
        if (isFlush && isStraight)
        {
            var flushRanks = flushCards.Select(c => (int)c.rank).ToList();
            if (flushRanks.Contains(14)) flushRanks.Add(1);

            for (int i = 0; i <= flushRanks.Count - 5; i++)
            {
                if (flushRanks[i] - 4 == flushRanks[i + 4])
                {
                    hand.Rank = flushRanks[i] == 14 ? HandRank.RoyalFlush : HandRank.StraightFlush;
                    hand.Kickers = new List<CardRank> { (CardRank)flushRanks[i] };
                    return hand;
                }
            }
        }

        // Каре
        var four = groupedByRank.FirstOrDefault(g => g.Count() == 4);
        if (four != null)
        {
            hand.Rank = HandRank.FourOfAKind;
            hand.Kickers = new List<CardRank> { four.Key };
            hand.Kickers.AddRange(groupedByRank.Where(g => g.Key != four.Key).Select(g => g.Key).OrderByDescending(k => k));
            return hand;
        }

        // Фулл хаус
        var three = groupedByRank.Where(g => g.Count() == 3).OrderByDescending(g => g.Key).ToList();
        var pair = groupedByRank.Where(g => g.Count() == 2).OrderByDescending(g => g.Key).ToList();
        if (three.Count > 0 && (pair.Count > 0 || three.Count > 1))
        {
            hand.Rank = HandRank.FullHouse;
            hand.Kickers = new List<CardRank> { three[0].Key, pair.Count > 0 ? pair[0].Key : three[1].Key };
            return hand;
        }

        // Флеш
        if (isFlush)
        {
            hand.Rank = HandRank.Flush;
            hand.Kickers = flushCards.Take(5).Select(c => c.rank).ToList();
            return hand;
        }

        // Стрейт
        if (isStraight)
        {
            hand.Rank = HandRank.Straight;
            hand.Kickers = new List<CardRank> { (CardRank)straightHigh };
            return hand;
        }

        // Сет
        if (three.Count > 0)
        {
            hand.Rank = HandRank.ThreeOfAKind;
            hand.Kickers = new List<CardRank> { three[0].Key };
            hand.Kickers.AddRange(groupedByRank.Where(g => g.Key != three[0].Key).Select(g => g.Key).OrderByDescending(k => k).Take(2));
            return hand;
        }

        // Две пары
        if (pair.Count >= 2)
        {
            hand.Rank = HandRank.TwoPair;
            hand.Kickers = pair.Take(2).Select(g => g.Key).ToList();
            hand.Kickers.AddRange(groupedByRank.Where(g => !hand.Kickers.Contains(g.Key)).Select(g => g.Key).OrderByDescending(k => k).Take(1));
            return hand;
        }

        // Одна пара
        if (pair.Count == 1)
        {
            hand.Rank = HandRank.OnePair;
            hand.Kickers = new List<CardRank> { pair[0].Key };
            hand.Kickers.AddRange(groupedByRank.Where(g => g.Key != pair[0].Key).Select(g => g.Key).OrderByDescending(k => k).Take(3));
            return hand;
        }

        // Старшая карта
        hand.Rank = HandRank.HighCard;
        hand.Kickers = allCards.Select(c => c.rank).OrderByDescending(r => r).Take(5).ToList();
        return hand;
    }

    public static int CompareHands(HandValue a, HandValue b)
    {
        if (a.Rank != b.Rank)
            return a.Rank.CompareTo(b.Rank);

        for (int i = 0; i < a.Kickers.Count; i++)
        {
            if (i >= b.Kickers.Count) return 1;
            int cmp = a.Kickers[i].CompareTo(b.Kickers[i]);
            if (cmp != 0) return cmp;
        }
        return 0;
    }
}
