using System.Collections.Generic;
using Unity.Netcode;
using Poker.Domain.Betting;

namespace Poker.Server
{
    /// <summary>
    /// Headless-сервер или локальный хост: хранит ставки текущего раунда.
    /// Полноценная очередь действий (SB/BB, Raise-loop) появится в v1.
    /// </summary>
    public sealed class TableServerController : NetworkBehaviour
    {
        private readonly List<PlayerBet> _currentRoundBets = new();

        /// <summary>Список ставок текущего betting-раунда.</summary>
        public IReadOnlyList<PlayerBet> CurrentRoundBets => _currentRoundBets;

        /// <summary>
        /// Регистрирует ставку от клиента/хоста. Пока сохраняем только факт;
        /// позже тут будет проверка стека, таймаутов и перехода улиц.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void RecordBetServerRpc(int playerId, BetAction action, int amount)
        {
            var bet = new Bet(action, amount);
            _currentRoundBets.Add(new PlayerBet(playerId, bet));
        }

        /// <summary>Очищает журнал ставок при начале новой улицы.</summary>
        public void BeginNewRound() => _currentRoundBets.Clear();
    }
}
