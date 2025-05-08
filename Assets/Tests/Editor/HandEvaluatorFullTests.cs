using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HandEvaluatorFullTests
{
    private CardDataSO MakeCard(CardRank rank, CardSuit suit)
    {
        var card = ScriptableObject.CreateInstance<CardDataSO>();
        card.rank = rank;
        card.suit = suit;
        return card;
    }

    [Test]
    public void Detect_OnePair()
    {
        var hole = new List<CardDataSO> { MakeCard(CardRank.King, CardSuit.Clubs), MakeCard(CardRank.King, CardSuit.Hearts) };
        var board = new List<CardDataSO>();
        var value = HandEvaluator.EvaluateHand(hole, board);
        Assert.AreEqual(HandRank.OnePair, value.Rank);
    }

    [Test]
    public void Detect_TwoPair()
    {
        var hole = new List<CardDataSO> { MakeCard(CardRank.King, CardSuit.Clubs), MakeCard(CardRank.Queen, CardSuit.Hearts) };
        var board = new List<CardDataSO> {
            MakeCard(CardRank.King, CardSuit.Diamonds),
            MakeCard(CardRank.Queen, CardSuit.Diamonds),
            MakeCard(CardRank.Two, CardSuit.Clubs),
        };
        var value = HandEvaluator.EvaluateHand(hole, board);
        Assert.AreEqual(HandRank.TwoPair, value.Rank);
    }

    [Test]
    public void Detect_ThreeOfAKind()
    {
        var hole = new List<CardDataSO> { MakeCard(CardRank.Ten, CardSuit.Clubs), MakeCard(CardRank.Ten, CardSuit.Hearts) };
        var board = new List<CardDataSO> { MakeCard(CardRank.Ten, CardSuit.Spades) };
        var value = HandEvaluator.EvaluateHand(hole, board);
        Assert.AreEqual(HandRank.ThreeOfAKind, value.Rank);
    }

    [Test]
    public void Detect_Straight()
    {
        var hole = new List<CardDataSO> { MakeCard(CardRank.Five, CardSuit.Clubs), MakeCard(CardRank.Six, CardSuit.Hearts) };
        var board = new List<CardDataSO> {
            MakeCard(CardRank.Seven, CardSuit.Diamonds),
            MakeCard(CardRank.Eight, CardSuit.Spades),
            MakeCard(CardRank.Nine, CardSuit.Hearts),
        };
        var value = HandEvaluator.EvaluateHand(hole, board);
        Assert.AreEqual(HandRank.Straight, value.Rank);
    }

    [Test]
    public void Detect_Flush()
    {
        var hole = new List<CardDataSO> { MakeCard(CardRank.Two, CardSuit.Hearts), MakeCard(CardRank.Four, CardSuit.Hearts) };
        var board = new List<CardDataSO> {
            MakeCard(CardRank.Six, CardSuit.Hearts),
            MakeCard(CardRank.Nine, CardSuit.Hearts),
            MakeCard(CardRank.Queen, CardSuit.Hearts),
        };
        var value = HandEvaluator.EvaluateHand(hole, board);
        Assert.AreEqual(HandRank.Flush, value.Rank);
    }

    [Test]
    public void Detect_FullHouse()
    {
        var hole = new List<CardDataSO> { MakeCard(CardRank.Jack, CardSuit.Hearts), MakeCard(CardRank.Jack, CardSuit.Spades) };
        var board = new List<CardDataSO> {
            MakeCard(CardRank.Jack, CardSuit.Diamonds),
            MakeCard(CardRank.Three, CardSuit.Clubs),
            MakeCard(CardRank.Three, CardSuit.Hearts),
        };
        var value = HandEvaluator.EvaluateHand(hole, board);
        Assert.AreEqual(HandRank.FullHouse, value.Rank);
    }

    [Test]
    public void Detect_FourOfAKind()
    {
        var hole = new List<CardDataSO> { MakeCard(CardRank.Nine, CardSuit.Spades), MakeCard(CardRank.Nine, CardSuit.Clubs) };
        var board = new List<CardDataSO> {
            MakeCard(CardRank.Nine, CardSuit.Diamonds),
            MakeCard(CardRank.Nine, CardSuit.Hearts),
        };
        var value = HandEvaluator.EvaluateHand(hole, board);
        Assert.AreEqual(HandRank.FourOfAKind, value.Rank);
    }

    [Test]
    public void Detect_StraightFlush()
    {
        var hole = new List<CardDataSO> { MakeCard(CardRank.Eight, CardSuit.Spades), MakeCard(CardRank.Seven, CardSuit.Spades) };
        var board = new List<CardDataSO> {
            MakeCard(CardRank.Six, CardSuit.Spades),
            MakeCard(CardRank.Five, CardSuit.Spades),
            MakeCard(CardRank.Four, CardSuit.Spades),
        };
        var value = HandEvaluator.EvaluateHand(hole, board);
        Assert.AreEqual(HandRank.StraightFlush, value.Rank);
    }

    [Test]
    public void Detect_RoyalFlush()
    {
        var hole = new List<CardDataSO> { MakeCard(CardRank.Ace, CardSuit.Hearts), MakeCard(CardRank.King, CardSuit.Hearts) };
        var board = new List<CardDataSO> {
            MakeCard(CardRank.Queen, CardSuit.Hearts),
            MakeCard(CardRank.Jack, CardSuit.Hearts),
            MakeCard(CardRank.Ten, CardSuit.Hearts),
        };
        var value = HandEvaluator.EvaluateHand(hole, board);
        Assert.AreEqual(HandRank.RoyalFlush, value.Rank);
    }
}
