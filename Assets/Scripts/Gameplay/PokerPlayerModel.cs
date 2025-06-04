using System;
using System.Collections.Generic;

/// <summary>
/// Модель игрока: стек, карты, состояние фолда и ставки.
/// Совместима со старым кодом через алиасы IsFolded/TotalBet.
/// </summary>
public class PokerPlayerModel
{
    public int Id { get; }
    public int Stack { get; private set; }

    // Новое имя свойства
    public bool HasFolded { get; private set; }

    // Старое имя — остаётся, чтобы не менять чужой код
    public bool IsFolded => HasFolded;

    public List<CardDataSO> HoleCards { get; } = new();

    /// <summary>Ставка в текущем BettingRound.</summary>
    public int CurrentBet { get; private set; }

    /// <summary>Суммарные вложения в раздаче (для сайд-потов).</summary>
    public int TotalBet { get; private set; }

    public PokerPlayerModel(int id, int stack)
    {
        Id = id;
        Stack = stack;
    }

    public bool TryBet(int amount)
    {
        if (HasFolded)
            return false;
        if (amount <= 0 || amount > Stack)
            return false;

        Stack -= amount;
        CurrentBet += amount;
        TotalBet += amount;
        return true;
    }

    public void Fold() => HasFolded = true;
    public void ResetForRound() => CurrentBet = 0;

    public void ResetForHand()
    {
        HasFolded = false;
        CurrentBet = 0;
        TotalBet = 0;
        HoleCards.Clear();
    }

    public void AddChips(int amount) => Stack += amount;

    /// <summary>Вызывает TryBet(amount) и кидает исключение, если неудачно.</summary>
    public void SpendChips(int amount)
    {
        if (!TryBet(amount))
            throw new System.InvalidOperationException(
                $"Player {Id} cannot spend {amount} (stack {Stack}, folded={HasFolded}).");
    }

    /// <summary>Выдаёт карту игроку (добавляет в HoleCards).</summary>
    public void DealCard(CardDataSO card)
    {
        if (card == null)
            throw new System.ArgumentNullException(nameof(card));
        HoleCards.Add(card);
    }

    /// <summary>Вызывается на начало betting round: обнуляет ставку.</summary>
    public void ResetRound()
    {
        ResetForRound();
    }
    
    public bool IsAllIn { get; private set; }

    public void GoAllIn()
    {
        if (Stack > 0)
        {
            TryBet(Stack);
            IsAllIn = true;
        }
    }


}
