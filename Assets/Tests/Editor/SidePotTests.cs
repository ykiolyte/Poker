using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SidePotTests
{
    private CardDataSO MakeCard(CardRank rank, CardSuit suit)
    {
        var card = ScriptableObject.CreateInstance<CardDataSO>();
        card.rank = rank;
        card.suit = suit;
        return card;
    }

    [Test]
    public void DistributeSidePot_SplitCorrectly()
    {
        var p1 = new PokerPlayerModel(1, 1000);
        var p2 = new PokerPlayerModel(2, 1000);
        var p3 = new PokerPlayerModel(3, 1000);

        p1.SpendChips(100); // All-in
        p2.SpendChips(200);
        p3.SpendChips(300);

        p1.HoleCards.Add(MakeCard(CardRank.Ace, CardSuit.Clubs));
        p1.HoleCards.Add(MakeCard(CardRank.Ace, CardSuit.Hearts));

        p2.HoleCards.Add(MakeCard(CardRank.King, CardSuit.Clubs));
        p2.HoleCards.Add(MakeCard(CardRank.King, CardSuit.Hearts));

        p3.HoleCards.Add(MakeCard(CardRank.Queen, CardSuit.Clubs));
        p3.HoleCards.Add(MakeCard(CardRank.Queen, CardSuit.Hearts));

        var board = new List<CardDataSO>();
        int pot = 100 + 200 + 300; // 600 total

        var result = WinnerDeterminer.DistributeSidePots(new List<PokerPlayerModel> { p1, p2, p3 }, board, pot);

        Assert.AreEqual(300, result[p1]); // p1 получает side pot (3 игроков по 100)
        Assert.AreEqual(200, result[p2]); // оставшийся средний банк
        Assert.AreEqual(100, result[p3]); // верхний банк

    }
}
