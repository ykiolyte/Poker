using NUnit.Framework;
using Poker.Domain.Betting;
using Poker.GameLoop;
using System.Collections.Generic;

public class PotManagerTests
{
    [Test]
    public void AllIn_SidePot_Distributed()
    {
        var pm = new PotManager();
        var A  = new PokerPlayerModel(0, 1000);
        var B  = new PokerPlayerModel(1, 1000);
        var C  = new PokerPlayerModel(2, 1000);

        pm.AddBet(A, 100);
        pm.AddBet(B, 400);
        pm.AddBet(C, 400);

        var hr = new Dictionary<int, HandResult>
        {
            [0] = new HandResult(new HandScore(Poker.GameLoop.HandRank.TwoPair,   new[]{14}), null),
            [1] = new HandResult(new HandScore(Poker.GameLoop.HandRank.HighCard,  new[]{13}), null),
            [2] = new HandResult(new HandScore(Poker.GameLoop.HandRank.Straight,  new[]{9}),  null),
        };

        var pay = pm.Distribute(hr);

        Assert.AreEqual(300, pay[0]); // main
        Assert.AreEqual(600, pay[2]); // side
    }
}
