using System;
using System.Collections.Generic; // для Math

public class PokerPlayerModel
{
    public int Id { get; }
    public int Stack { get; private set; }
    public bool IsFolded { get; private set; }
    public List<CardDataSO> HoleCards { get; } = new();
    public int TotalBet { get; set; } // добавлено для side-pot логики

    public PokerPlayerModel(int id, int stack)
    {
        Id = id;
        Stack = stack;
        IsFolded = false;
    }

    public void Fold() => IsFolded = true;

    public bool SpendChips(int amount)
    {
        if (amount > Stack) return false;
        Stack -= amount;
        TotalBet += amount;
        return true;
    }

    public void AddChips(int amount)
    {
        Stack += amount;
    }

    public void DealCard(CardDataSO card)
    {
        HoleCards.Add(card);
    }

    public void ResetRound()
    {
        IsFolded = false;
        HoleCards.Clear();
        TotalBet = 0;
    }
}
