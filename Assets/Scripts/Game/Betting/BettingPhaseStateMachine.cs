// Assets/Scripts/Game/Betting/BettingPhaseStateMachine.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Poker.Infrastructure.Timers;
using Poker.GameLoop;
using Poker.Domain.Betting;
using Poker.UI;
using Poker.Gameplay;
using Poker.Core.Config;

namespace Poker.Game.Betting
{
    public sealed class BettingPhaseStateMachine
    {
        public event Action<PokerPlayerModel, BettingAction> PlayerActed;
        public event Action<List<PlayerBet>> RoundCompleted;

        private readonly IList<PokerPlayerController> controllers;
        private readonly int smallBlind;
        private readonly int bigBlind;
        private readonly Street street;
        private readonly BettingTimerService timer;
        private readonly GameUIController ui;
        private readonly GameSettingsSO settings;

        private readonly List<PlayerBet> actions = new();
        private int currentBet;
        private bool waiting;
        private readonly float turnSeconds = 15f;

        public BettingPhaseStateMachine(
            IList<PokerPlayerController> controllers,
            int smallBlind,
            int bigBlind,
            Street street,
            BettingTimerService timer,
            GameUIController uiController,
            GameSettingsSO settings)
        {
            this.controllers = controllers;
            this.smallBlind  = smallBlind;
            this.bigBlind    = bigBlind;
            this.street      = street;
            this.timer       = timer;
            this.ui          = uiController;
            this.settings    = settings;
        }

        public IEnumerator Execute()
        {
            if (street == Street.Preflop)
                currentBet = settings.BigBlind;

            int idx = 0;
            while (ActivePlayers() > 1 && !EveryoneCalled())
            {
                var c = controllers[idx];
                if (!c.Model.HasFolded && !c.Model.IsAllIn)
                {
                    yield return TakeTurn(c);

                    var action = c.LastAction;
                    actions.Add(new PlayerBet(c.Model.Id, ConvertToBet(action)));
                    PlayerActed?.Invoke(c.Model, action);

                    if (action.ActionType == BettingActionType.Raise)
                    {
                        currentBet = action.Amount;
                        ui.SetTableBet(currentBet);
                    }
                }

                idx = (idx + 1) % controllers.Count;
            }

            RoundCompleted?.Invoke(actions);
        }

        private IEnumerator TakeTurn(PokerPlayerController c)
        {
            waiting = true;
            timer.StartTimer(turnSeconds);
            ui.UpdateTurnTimer(1f);
            ui.SetPlayerTurn($"P#{c.Model.Id}");
            c.GetComponent<PlayerHighlightView>()?.SetHighlight(true);

            timer.TimerTick += OnTick;
            timer.TimerExpired += () =>
            {
                var canCheck = currentBet <= c.Model.CurrentBet;
                if (canCheck)
                    c.LastAction = new BettingAction(BettingActionType.Check);
                else
                    c.ForceFold();
                waiting = false;
            };

            int diff = currentBet - c.Model.CurrentBet;

            if (c.Model.Stack == 0)
            {
                c.LastAction = new BettingAction(BettingActionType.Check);
                waiting = false;
            }
            else if (c.Model.Stack <= diff)
            {
                c.Model.GoAllIn();
                c.LastAction = new BettingAction(BettingActionType.Call, diff);
                waiting = false;
            }
            else
            {
                c.RequestBet(currentBet, () => waiting = false);
            }

            while (waiting) yield return null;

            timer.TimerTick -= OnTick;
            timer.StopTimer();
            c.GetComponent<PlayerHighlightView>()?.SetHighlight(false);

            void OnTick(float t01) => ui.UpdateTurnTimer(t01);
        }

        private bool EveryoneCalled()
        {
            foreach (var c in controllers)
                if (!c.Model.HasFolded && !c.Model.IsAllIn && c.Model.CurrentBet < currentBet)
                    return false;
            return true;
        }

        private int ActivePlayers()
        {
            int count = 0;
            foreach (var c in controllers)
                if (!c.Model.HasFolded) count++;
            return count;
        }

        private Bet ConvertToBet(BettingAction action) => action.ActionType switch
        {
            BettingActionType.Check => new Bet(BetAction.Check),
            BettingActionType.Call  => new Bet(BetAction.Call,  action.Amount),
            BettingActionType.Raise => new Bet(BetAction.Raise, action.Amount),
            BettingActionType.Fold  => new Bet(BetAction.Fold),
            BettingActionType.AllIn => new Bet(BetAction.Raise, action.Amount),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
