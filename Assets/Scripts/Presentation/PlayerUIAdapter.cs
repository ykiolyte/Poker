using System;
using Poker.Gameplay;
using Poker.Game.Betting;   // ← нужен ActionType / PlayerAction
using UnityEngine;
using Poker.UI;

namespace Poker.Presentation
{
    public class PlayerUIAdapter : MonoBehaviour, IPlayerInputPresenter
    {
        [SerializeField] private PlayerUIController controller;

        public void QueryAction(PlayerContext ctx, Action<PlayerAction> cb)
        {
            controller.RequestAction(
                ctx.MaxBet,
                1000,                                  // mock stack
                betting => cb(Convert(betting)));
        }

        private PlayerAction Convert(BettingAction b, int tableBet = 0) => b.ActionType switch
        {
            BettingActionType.Fold  => PlayerAction.Fold(),
            BettingActionType.Call  => PlayerAction.Call(),
            BettingActionType.Check => PlayerAction.Check(),
            BettingActionType.AllIn => PlayerAction.Raise(int.MaxValue),
            BettingActionType.Raise => PlayerAction.Raise(b.Amount),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
