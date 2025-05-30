using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
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

    [Header("Raise Input")]
    public TMP_InputField raiseInputField;
    public Button confirmRaiseButton;

    [Header("Player Cards")]
    public Image card1Image;
    public Image card2Image;

    [Header("Game Status")]
    public Text gamePhaseText;
    public Text playerMessageText;

    public void SetPotAmount(int amount)
    {
        potText.text = $"Bank: {amount}";
    }

    public void SetPlayerTurn(string playerName)
    {
        playerTurnText.text = $"Your turn: {playerName}";
    }

    public void SetActionButtonsActive(bool isActive)
    {
        raiseButton.interactable = isActive;
        callButton.interactable = isActive;
        foldButton.interactable = isActive;
        checkButton.interactable = isActive;
        allInButton.interactable = isActive;
    }

    public void SetGamePhase(string phase)
{
    gamePhaseText.text = $"Phase: {phase}";
}

    public void ShowPlayerMessage(string message, float duration = 2f)
    {
        playerMessageText.text = message;
        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), duration);
    }

    private void ClearMessage() => playerMessageText.text = "";
}
