using System.Collections.Generic;
using Poker.Core.Config;

public class BettingSystem
{
    private readonly List<PokerPlayerModel> players;
    private readonly GameSettingsSO settings;
    private int pot;
    private int currentBet;

    public int Pot => pot;
    public int CurrentBet => currentBet;

    public BettingSystem(List<PokerPlayerModel> players, GameSettingsSO settings)
    {
        this.players = players;
        this.settings = settings;
        pot = 0;
        currentBet = settings.BigBlind;
    }

    public bool PerformAction(PokerPlayerModel player, BettingAction action)
    {
        switch (action.ActionType)
        {
            case BettingActionType.Check:
                if (currentBet == 0) return true;
                return false;

            case BettingActionType.Call:
                return Call(player);

            case BettingActionType.Raise:
                return Raise(player, action.Amount);

            case BettingActionType.AllIn:
                return AllIn(player);

            case BettingActionType.Fold:
                player.Fold();
                return true;
        }
        return false;
    }

    private bool Call(PokerPlayerModel player)
    {
        int diff = currentBet;
        if (player.Stack < diff) return false;
        if (!player.SpendChips(diff)) return false;
        pot += diff;
        return true;
    }

    private bool Raise(PokerPlayerModel player, int amount)
    {
        int total = currentBet + amount;
        if (!player.SpendChips(total)) return false;
        currentBet = total;
        pot += total;
        return true;
    }

    private bool AllIn(PokerPlayerModel player)
    {
        int amount = player.Stack;
        if (!player.SpendChips(amount)) return false;
        pot += amount;
        return true;
    }
}
