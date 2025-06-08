using System;
using Poker.Gameplay;
using Poker.Game.Betting;
using Poker.Presentation;
using UnityEngine;
using Poker.UI;

namespace Poker.Presentation
{
    /// <summary>Адаптирует PlayerUIController под IPlayerInputPresenter.</summary>
    public class PlayerUIAdapter : MonoBehaviour, IPlayerInputPresenter
    {
        [SerializeField] private PlayerUIController controller;

        public void QueryAction(PlayerContext context, Action<PlayerAction> callback)
        {
            controller.RequestAction(
                context.MaxBet,
                1000, // TODO: заменить на реальный стек игрока
                betting =>
                {
                    var playerAction = ConvertToPlayerAction(betting, context.MaxBet);
                    callback(playerAction);
                });
        }

        private PlayerAction ConvertToPlayerAction(BettingAction betting, int currentMaxBet)
        {
            return betting.ActionType switch
            {
                BettingActionType.Fold  => PlayerAction.Fold(),
                BettingActionType.Call  => PlayerAction.Call(),
                BettingActionType.Check => PlayerAction.Check(),
                BettingActionType.AllIn => PlayerAction.Raise(int.MaxValue),
                BettingActionType.Raise => PlayerAction.Raise(betting.Amount),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
