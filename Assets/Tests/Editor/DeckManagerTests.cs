using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManagerTests
{
    [Test]
    public void Deck_Initialization_HasCorrectCount()
    {
        var cardData = ScriptableObject.CreateInstance<CardDataSO>();
        var deckManager = new DeckManager();
        deckManager.InitializeDeck(Enumerable.Repeat(cardData, 52).ToArray());
        Assert.AreEqual(52, deckManager.DrawCardCount());
    }

    [Test]
    public void Deck_DrawCard_RemovesCard()
    {
        var card = ScriptableObject.CreateInstance<CardDataSO>();
        var deckManager = new DeckManager();
        deckManager.InitializeDeck(new[] { card });
        var drawn = deckManager.DrawCard();
        Assert.AreEqual(card, drawn);
        Assert.IsNull(deckManager.DrawCard());
    }

    [Test]
    public void Deck_Shuffle_ChangesOrder()
    {
        var cardData = Enumerable.Range(0, 10).Select(i => {
            var card = ScriptableObject.CreateInstance<CardDataSO>();
            card.rank = (CardRank)(i + 2);
            return card;
        }).ToArray();

        var deckManager = new DeckManager();
        deckManager.InitializeDeck(cardData);

        var original = cardData.Select(c => c.rank).ToList();
        deckManager.Shuffle();
        var shuffled = new List<CardRank>();
        while (deckManager.DrawCardCount() > 0)
            shuffled.Add(deckManager.DrawCard().rank);

        Assert.AreNotEqual(original, shuffled); // возможен редкий false positive
    }

    [Test]
    public void Deck_DrawAll_NoDuplicates()
    {
        var cardData = Enumerable.Range(0, 52).Select(i => {
            var card = ScriptableObject.CreateInstance<CardDataSO>();
            card.rank = (CardRank)((i % 13) + 2);
            card.suit = (CardSuit)(i / 13);
            return card;
        }).ToArray();

        var deckManager = new DeckManager();
        deckManager.InitializeDeck(cardData);
        deckManager.Shuffle();

        var drawn = new HashSet<string>();
        while (deckManager.DrawCardCount() > 0)
        {
            var card = deckManager.DrawCard();
            var id = $"{card.rank}-{card.suit}";
            Assert.IsFalse(drawn.Contains(id));
            drawn.Add(id);
        }
        Assert.AreEqual(52, drawn.Count);
    }
}
