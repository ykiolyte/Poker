// Assets/Scripts/UI/PlayerUIController.cs
using System;
using UnityEngine;
using Poker.Game.Betting;

namespace Poker.UI
{
    public sealed class PlayerUIController : MonoBehaviour
    {
        [SerializeField] private GameUIController ui;
        [SerializeField] private int bigBlind = 50;          // настроить в инспекторе

        Action<BettingAction> _onDecision;
        int  _tableBet;

        public void Initialize(GameUIController c) => ui = c;

        /* ---------- Start: подписка на кнопки ----------------- */
        void Start()
        {
            if (ui == null) { enabled = false; return; }

            ui.raiseButton .onClick.AddListener(OnRaiseClicked);
            ui.callButton  .onClick.AddListener(() => Decide(BettingActionType.Call));
            ui.foldButton  .onClick.AddListener(() => Decide(BettingActionType.Fold));
            ui.checkButton .onClick.AddListener(() => Decide(BettingActionType.Check));
            ui.allInButton .onClick.AddListener(() => Decide(BettingActionType.AllIn));
            ui.confirmRaiseButton.onClick.AddListener(ConfirmRaise);

            ui.SetActionButtonsActive(false);
            ui.raiseInputField.gameObject.SetActive(false);
            ui.confirmRaiseButton.gameObject.SetActive(false);
        }

        /* ---------- API из PokerPlayerController --------------- */
        public void RequestAction(int tableBet, int stack, Action<BettingAction> cb)
        {
            _onDecision = cb;
            _tableBet   = tableBet;

            ui.SetTableBet(tableBet);
            ui.raiseInputField.text = "";
            ui.SetActionButtonsActive(true);

            ui.callButton .gameObject.SetActive(stack >= tableBet);
            ui.checkButton.gameObject.SetActive(tableBet == 0);
            ui.raiseButton.gameObject.SetActive(stack > tableBet);
            ui.allInButton.gameObject.SetActive(stack > 0);
        }

        /* ---------- кнопки ------------------------------------ */
        void OnRaiseClicked()
        {
            ui.raiseInputField.gameObject.SetActive(true);
            ui.confirmRaiseButton.gameObject.SetActive(true);
        }

        void ConfirmRaise()
        {
            if (!int.TryParse(ui.raiseInputField.text, out var amount) || amount <= 0)
            {
                ui.ShowPlayerMessage("Введите число");
                return;
            }

            int minRaise = Mathf.Max(bigBlind * 2, _tableBet * 2);
            if (amount < minRaise)
            {
                ui.ShowPlayerMessage($"Мин. рейз {minRaise}");
                return;
            }

            Decide(BettingActionType.Raise, amount);
        }

        void Decide(BettingActionType type, int amount = 0)
        {
            ui.SetActionButtonsActive(false);
            ui.raiseInputField   .gameObject.SetActive(false);
            ui.confirmRaiseButton.gameObject.SetActive(false);

            _onDecision?.Invoke(new BettingAction(type, amount));
            _onDecision = null;
        }
    }
}
