// Assets/Scripts/GameLoop/BettingSystem.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poker.Domain.Betting;
using Poker.Game.Betting;
using Poker.Core.Config;
using Poker.Infrastructure.Timers;
using Poker.UI;

namespace Poker.GameLoop
{
    [RequireComponent(typeof(BettingTimerService))]
    public class BettingSystem : MonoBehaviour
    {
        [SerializeField] private GameSettingsSO settings;
        [SerializeField] private GameUIController ui;  // ← добавляем UI

        private BettingTimerService timer;
        private readonly BetHistory history = new();

        void Awake() => timer = GetComponent<BettingTimerService>();

        public IEnumerator ExecuteBettingRound(List<PokerPlayerController> controllers, Street street)
        {
            var fsm = new BettingPhaseStateMachine(
                controllers,
                settings.SmallBlind,
                settings.BigBlind,
                street,
                timer,
                ui); // ← теперь передаём UI

            fsm.RoundCompleted += round =>
            {
                history.AddRound(new BetRound(street, new List<PlayerBet>(round)));
                Debug.Log($"[Betting] {street} round complete – {round.Count} actions");
            };

            yield return fsm.Execute();
        }

        public BetHistory History => history;
    }
}
