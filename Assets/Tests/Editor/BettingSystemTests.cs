using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BettingSystemTests
{
    private PokerPlayerModel player;
    private GameSettingsSO settings;
    private BettingSystem betting;

    [SetUp]
    public void Setup()
    {
        player = new PokerPlayerModel(1, 1000);
        settings = ScriptableObject.CreateInstance<GameSettingsSO>();
        settings.bigBlind = 100;
        betting = new BettingSystem(new List<PokerPlayerModel> { player }, settings);
    }

    [Test]
    public void Perform_Call_DecreasesStack()
    {
        var result = betting.PerformAction(player, new BettingAction(BettingActionType.Call));
        Assert.IsTrue(result);
        Assert.AreEqual(900, player.Stack);
        Assert.AreEqual(100, betting.Pot);
    }

    [Test]
    public void Perform_Raise_SetsCurrentBet()
    {
        var result = betting.PerformAction(player, new BettingAction(BettingActionType.Raise, 200));
        Assert.IsTrue(result);
        Assert.AreEqual(700, player.Stack);
        Assert.AreEqual(300, betting.CurrentBet);
    }

    [Test]
    public void Perform_AllIn_EmptiesStack()
    {
        var result = betting.PerformAction(player, new BettingAction(BettingActionType.AllIn));
        Assert.IsTrue(result);
        Assert.AreEqual(0, player.Stack);
    }
}
