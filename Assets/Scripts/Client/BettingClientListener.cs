using UnityEngine;
using System.Linq;
using Poker.UI;
using Poker.Game.Betting;
using Poker.Gameplay;
using Poker.GameLoop;

/// <summary>
/// Реагирует на события ставок и выигрышей — обновляет UI и состояния игроков.
/// Подписан на RPC от NetworkBettingEngine.
/// </summary>
public sealed class BettingClientListener : MonoBehaviour
{
    [SerializeField] private GameUIController ui;

    /// <summary>
    /// Обрабатывает событие ставки или фолда, обновляет UI и модель игрока.
    /// </summary>
    public void OnActionApplied(int seat, ActionType type, int amount)
    {
        var pc = FindPlayer(seat);
        if (pc == null)
        {
            Debug.LogWarning($"[BettingClientListener] Игрок с seat={seat} не найден.");
            return;
        }

        pc.LastAction = new BettingAction(Convert(type), amount);

        if (type is ActionType.Raise or ActionType.Call)
            ui.SetTableBet(amount);
    }

    /// <summary>
    /// Отображает сообщение о выигрыше игрока.
    /// </summary>
    public void ShowWinner(int seat, int amount)
    {
        if (ui == null)
        {
            Debug.LogError("[BettingClientListener] UI-ссылка не установлена.");
            return;
        }

        ui.ShowPlayerMessage($"Player #{seat} wins {amount} chips", 3f);
    }

    /// <summary>
    /// Находит PokerPlayerController по seat.
    /// </summary>
    private PokerPlayerController FindPlayer(int seat) =>
        FindObjectsByType<PokerPlayerController>(FindObjectsSortMode.None)
            .FirstOrDefault(p => p.Model.Id == seat);

    /// <summary>
    /// Конвертирует ActionType (из сети) в BettingActionType (в UI).
    /// </summary>
    private BettingActionType Convert(ActionType t) => t switch
    {
        ActionType.Fold  => BettingActionType.Fold,
        ActionType.Check => BettingActionType.Check,
        ActionType.Call  => BettingActionType.Call,
        ActionType.Raise => BettingActionType.Raise,
        _ => BettingActionType.None
    };
}
