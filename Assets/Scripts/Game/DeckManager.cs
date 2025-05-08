using System.Collections.Generic;
using UnityEngine;

public class DeckManager
{
    private readonly List<CardDataSO> deck = new();
    private readonly System.Random rng = new();

    public void InitializeDeck(CardDataSO[] cardData)
    {
        deck.Clear();
        deck.AddRange(cardData);
    }

    public void Shuffle()
    {
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (deck[k], deck[n]) = (deck[n], deck[k]);
        }
    }

    public CardDataSO DrawCard()
    {
        if (deck.Count == 0) return null;
        var card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    public int DrawCardCount() => deck.Count;
}