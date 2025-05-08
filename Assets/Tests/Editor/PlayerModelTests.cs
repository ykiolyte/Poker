using NUnit.Framework;

public class PlayerModelTests
{
    [Test]
    public void Player_InitialStack_IsCorrect()
    {
        var player = new PokerPlayerModel(0, 1000);
        Assert.AreEqual(1000, player.Stack);
    }

    [Test]
    public void Player_Fold_SetsFlag()
    {
        var player = new PokerPlayerModel(1, 1000);
        player.Fold();
        Assert.IsTrue(player.IsFolded);
    }

    [Test]
    public void Player_CanSpendAndAddChips()
    {
        var player = new PokerPlayerModel(2, 1000);
        Assert.IsTrue(player.SpendChips(200));
        Assert.AreEqual(800, player.Stack);
        player.AddChips(100);
        Assert.AreEqual(900, player.Stack);
    }
}