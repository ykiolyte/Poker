// Assets/Scripts/UI/GameUIController.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poker.UI
{
    public sealed class GameUIController : MonoBehaviour
    {
        [Header("Action Buttons")]
        public Button raiseButton;
        public Button callButton;
        public Button foldButton;
        public Button checkButton;
        public Button allInButton;

        [Header("Text Displays")]
        public Text potText;
        public Text playerTurnText;
        public Text tableBetText;            // ← NEW

        [Header("Raise Input")]
        public TMP_InputField raiseInputField;
        public Button confirmRaiseButton;

        [Header("Game Status")]
        public Text gamePhaseText;
        public Text playerMessageText;

        [Header("Timer")]
        public Image timerFillImage;         // radial fill 0-1
        public float  lowTimeThreshold = .25f;
        public Color  normalColor  = Color.green;
        public Color  warningColor = Color.red;

        /* ---------- public API ------------------------------------------------ */

        public void SetPotAmount(int amount)          => potText.text      = $"Bank: {amount}";
        public void SetPlayerTurn(string name)        => playerTurnText.text = $"Your turn: {name}";
        public void SetTableBet(int amount)           => tableBetText.text = $"Current bet: {amount}";
        public void SetGamePhase(string phase)        => gamePhaseText.text = $"Phase: {phase}";

        public void SetActionButtonsActive(bool on)
        {
            raiseButton.interactable =
            callButton .interactable =
            foldButton .interactable =
            checkButton.interactable =
            allInButton.interactable = on;
        }

        public void UpdateTurnTimer(float t01)
        {
            timerFillImage.fillAmount = t01;
            timerFillImage.color      = (t01 < lowTimeThreshold) ? warningColor : normalColor;
        }

        public void ShowPlayerMessage(string msg, float duration = 2f)
        {
            playerMessageText.text = msg;
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), duration);
        }

        /* ---------- private --------------------------------------------------- */
        void ClearMessage() => playerMessageText.text = "";
    }
}
