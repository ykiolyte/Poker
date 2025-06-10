using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Poker.Core.Config;
using Poker.Domain.Betting;
using Poker.GameLoop;

namespace Poker.Server
{
    [RequireComponent(typeof(NetworkObject))]
    public sealed class TableServerController : NetworkBehaviour
    {
        [SerializeField] private GameSettingsSO gameSettings;

        private readonly List<PlayerBet> _currentRound = new();
        private readonly PotManager      _pots         = new();  // NEW

        public PotManager Pots => _pots;
        public IReadOnlyList<PlayerBet> CurrentRoundBets => _currentRound;

        /* ---------- ставки / блайнды ---------- */

        [ServerRpc(RequireOwnership = false)]
        public void PostBlindsServerRpc(int sbSeat, int bbSeat)
        {
            _pots.AddBet(FindModel(sbSeat), gameSettings.SmallBlind);
            _pots.AddBet(FindModel(bbSeat), gameSettings.BigBlind);
            // (Broadcast клиенты — оставил прежний)
        }

        [ServerRpc(RequireOwnership = false)]
        public void RecordBetServerRpc(int seat, BetAction act, int amount)
        {
            var p = FindModel(seat);
            if (p == null) return;

            _currentRound.Add(new PlayerBet(seat, new Bet(act, amount)));
            if (act is BetAction.Call or BetAction.Raise)
                _pots.AddBet(p, amount);
        }

        public void BeginNewRound()
        {
            _pots.Reset();
            _currentRound.Clear();
        }

        /* ---------- SHOWDOWN --------------- */

        public void FinishHandShowdown()
        {
            if (!IsServer) return;

            var handResults = CollectHands();
            var payout      = _pots.Distribute(handResults);

            foreach (var kv in payout)
                FindModel(kv.Key)?.AddChips(kv.Value);

            WinningsClientRpc(payout.Keys.ToArray(), payout.Values.ToArray());
        }

        [ClientRpc]
        void WinningsClientRpc(int[] seats, int[] amounts)
        {
            for (int i = 0; i < seats.Length; i++)
            {
                foreach (var l in FindObjectsByType<BettingClientListener>(FindObjectsSortMode.None))
                    l.ShowWinner(seats[i], amounts[i]);
            }
        }

        /* ---------- helpers ---------------- */

        private Dictionary<int,HandResult> CollectHands()
        {
            var rm = FindFirstObjectByType<RoundManager>();
            var map = new Dictionary<int,HandResult>();

            foreach (var pc in FindObjectsByType<PokerPlayerController>(FindObjectsSortMode.None))
                if (!pc.Model.HasFolded)
                    map[pc.Model.Id] = HandEvaluator.EvaluateBestHand(
                        pc.Model.HoleCards.ToList(), rm.BoardCards);

            return map;
        }

        private PokerPlayerModel FindModel(int seat) =>
            FindObjectsByType<PokerPlayerController>(FindObjectsSortMode.None)
                .FirstOrDefault(p => p.Model.Id == seat)?.Model;
    }
}
