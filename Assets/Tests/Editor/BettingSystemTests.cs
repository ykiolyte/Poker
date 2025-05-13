using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Poker.Core.Config;
using Tests.Testables;

namespace Tests.Editor
{
    public class BettingSystemTests
    {
        private PokerPlayerModel player;
        private GameSettingsSO settings;
        private BettingSystem betting;

        [SetUp]
        public void Setup()
        {
            // Создаём тестовый ScriptableObject с возможностью задания параметров
            var testSettings = ScriptableObject.CreateInstance<TestableGameSettingsSO>();
            testSettings.Init(small: 50, big: 100, chips: 1000, max: 6);
            settings = testSettings;

            // Создаём игрока с начальным стеком
            player = new PokerPlayerModel(1, testSettings.StartingChips);

            // Передаём игроков и настройки в BettingSystem
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
            Assert.AreEqual(300, betting.CurrentBet); // 100 call + 200 raise
        }

        [Test]
        public void Perform_AllIn_EmptiesStack()
        {
            var result = betting.PerformAction(player, new BettingAction(BettingActionType.AllIn));
            Assert.IsTrue(result);
            Assert.AreEqual(0, player.Stack);
        }
    }
}
