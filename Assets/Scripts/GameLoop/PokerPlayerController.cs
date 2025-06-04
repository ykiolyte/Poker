// Assets/Scripts/GameLoop/PokerPlayerController.cs
using System;
using UnityEngine;
using Poker.Domain.Betting;
using Poker.Game.Betting;
using Poker.UI;                       // ← новый using

namespace Poker.GameLoop
{
    /// <summary>
    /// Контроллер игрока — мост между Domain-логикой и представлением (UI, анимации).
    /// </summary>
    public sealed class PokerPlayerController : MonoBehaviour
    {
        // ─────────────────────── локальность ───────────────────────
        private bool _isLocalPlayer;
        public  bool IsLocalPlayer => _isLocalPlayer;

        [Header("Model & Anchors")]
        [SerializeField] private Transform leftCardAnchor;
        [SerializeField] private Transform rightCardAnchor;

        public PokerPlayerModel Model { get; private set; }

        /// <summary>Ссылка на UI-презентер (назначается Binder’ом).</summary>
        private PlayerUIController _ui;

        /// <summary>Последнее действие игрока в текущем betting-раунде.</summary>
        public BettingAction LastAction { get; internal set; }

        /// <summary>Уже сходил в этом раунде?</summary>
        public bool HasActedThisRound { get; private set; }

        //────────────────────────── Mono ──────────────────────────
        private void Awake()
        {
            // если Binder добавил компонент позже, всё равно найдём
            _ui = GetComponent<PlayerUIController>();
        }

        //────────────────────────── API ────────────────────────────
        public void InjectModel(PokerPlayerModel model) => Model = model;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [ContextMenu("Force Local Player")]
#endif
        public void SetAsLocalPlayer()
        {
            _isLocalPlayer = true;
            // может случиться, что UI добавлен уже после Awake
            _ui ??= GetComponent<PlayerUIController>();
        }

        /// <summary>
        /// Запрашивается BettingStateMachine, когда приходит ход игрока.
        /// Локальный игрок → показываем UI, ждём решения.
        /// Бот/удалённый игрок → auto-решение как раньше.
        /// </summary>
        public void RequestBet(int tableBet, Action onDecided)
        {
            if (_isLocalPlayer && _ui != null)
            {
                _ui.RequestAction(tableBet, Model.Stack, action =>
                {
                    ApplyAction(action, tableBet);
                    HasActedThisRound = true;
                    onDecided?.Invoke();
                });
                return; // ждём взаимодействия пользователя
            }

            // 💡 не локальный — instant decision
            var auto = AutoDecision(tableBet);
            ApplyAction(auto, tableBet);
            HasActedThisRound = true;
            onDecided?.Invoke();
        }

        /// <summary>Авто-Fold, когда таймер истёк.</summary>
        public void ForceFold()
        {
            Model.Fold();
            LastAction = new BettingAction(BettingActionType.Fold);
            HasActedThisRound = true;
        }

        public void ResetForNewHand()
        {
            Model.ResetForHand();
            HasActedThisRound = false;
            LastAction = null;
        }

        public Transform GetCardAnchor(bool isLeftHand) =>
            isLeftHand ? leftCardAnchor : rightCardAnchor;

        //────────────────────────── helpers ─────────────────────────

        private BettingAction AutoDecision(int tableBet)
        {
            int diff = tableBet - Model.CurrentBet;

            if (diff <= 0)
                return new BettingAction(BettingActionType.Check);

            if (Model.Stack >= diff)
                return new BettingAction(BettingActionType.Call, diff);

            return new BettingAction(BettingActionType.Fold);
        }

       private void ApplyAction(BettingAction action, int tableBet)
        {
            int betAmount = 0;

            switch (action.ActionType)
            {
                case BettingActionType.Check:
                    Model.TryBet(0);
                    break;

                case BettingActionType.Call:
                    betAmount = tableBet - Model.CurrentBet;
                    Model.TryBet(betAmount);
                    break;

                case BettingActionType.Raise:
                    Model.TryBet(action.Amount);
                    betAmount = action.Amount;
                    break;

                case BettingActionType.AllIn:
                    betAmount = Model.Stack;
                    Model.TryBet(betAmount);
                    break;

                case BettingActionType.Fold:
                    Model.Fold();
                    break;
            }

            // фиксируем правильное действие
            LastAction = new BettingAction(action.ActionType, betAmount);
        }

    }
}
