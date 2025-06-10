using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Poker.Core.Config;
using Poker.Domain.Betting;
using Poker.Game.Betting;
using Poker.GameLoop;
using Poker.Gameplay;
using Street = Poker.Gameplay.Street;   // убираем двусмысленность

namespace Poker.Server
{
    [RequireComponent(typeof(NetworkObject))]
    public sealed class NetworkBettingEngine : NetworkBehaviour
    {
        [SerializeField] private GameSettingsSO       settings;
        [SerializeField] private TableServerController table;

        /* -------- состояние улицы -------- */
        private Street currentStreet = Street.PreFlop;

        /* -------- очередь ходов / ставки -------- */
        private readonly List<ulong> turnOrder = new();
        private int  currentIdx;
        private int  currentBet;
        private bool bettingActive;

        private readonly Dictionary<ulong,int> contributions = new();
        private readonly HashSet<ulong> folded = new();
        private readonly HashSet<ulong> allIn  = new();

        /* ---------------- API ---------------- */

        public void BeginStreet()
        {
            if (!IsServer) return;

            turnOrder.Clear(); contributions.Clear();
            folded.Clear();    allIn.Clear();

            foreach (var c in NetworkManager.Singleton.ConnectedClientsList)
                turnOrder.Add(c.ClientId);

            currentBet    = currentStreet == Street.PreFlop ? settings.BigBlind : 0;
            currentIdx    = 0;
            bettingActive = true;

            AskCurrentPlayer();
        }

        [ServerRpc(RequireOwnership = false)]
        public void SubmitActionServerRpc(ActionType t, int amt, ServerRpcParams rpc = default)
        {
            if (!bettingActive) return;
            ulong cid = rpc.Receive.SenderClientId;
            if (cid != turnOrder[currentIdx]) return;

            var model = FindModel(cid);
            if (model == null || !Validate(model, t, amt)) return;

            ApplyAction(cid, t, amt);
            BroadcastAction(model.Id, t, amt);

            if (!CheckStreetEnd()) { AdvanceTurn(); AskCurrentPlayer(); }
        }

        public void ReceiveRemoteAction(int seat, ActionType t, int amt)
        {
            if (!IsServer || !bettingActive) return;

            var cid = GetClientIdBySeat(seat);
            if (cid == null || cid != turnOrder[currentIdx]) return;

            var model = FindModel(cid.Value);
            if (model == null || !Validate(model, t, amt)) return;

            ApplyAction(cid.Value, t, amt);
            BroadcastAction(seat, t, amt);

            if (!CheckStreetEnd()) { AdvanceTurn(); AskCurrentPlayer(); }
        }

        /* ------------- BroadCast ------------- */

        void BroadcastAction(int seat, ActionType t, int amt)
        {
            foreach (var l in FindObjectsByType<BettingClientListener>(FindObjectsSortMode.None))
                l.OnActionApplied(seat, t, amt);

            foreach (var g in FindObjectsByType<RemotePlayerGateway>(FindObjectsSortMode.None))
                g.ActionAppliedClientRpc(seat, (int)t, amt);
        }

        /* ------------- Утилиты хода ----------- */

        void AskCurrentPlayer()
        {
            var gw = FindGateway(turnOrder[currentIdx]);
            gw?.RequestAction(new PlayerContext(currentBet, currentStreet), _ => { });
        }

        void AdvanceTurn()
        {
            do { currentIdx = (currentIdx + 1) % turnOrder.Count; }
            while (folded.Contains(turnOrder[currentIdx]) || allIn.Contains(turnOrder[currentIdx]));
        }

        /* ------------- Валидация / применение ---------- */

        bool Validate(PokerPlayerModel m, ActionType t, int amt)
        {
            int diff = currentBet - m.CurrentBet;
            return t switch
            {
                ActionType.Fold  => true,
                ActionType.Check => diff == 0,
                ActionType.Call  => m.Stack >= diff,
                ActionType.Raise => amt >= currentBet * 2 && amt <= m.Stack + m.CurrentBet,
                _ => false
            };
        }

        void ApplyAction(ulong cid, ActionType t, int amt)
        {
            var m = FindModel(cid);

            switch (t)
            {
                case ActionType.Fold:
                    folded.Add(cid);
                    m.Fold();
                    break;

                case ActionType.Call:
                    table.Pots.AddBet(m, currentBet - m.CurrentBet);
                    contributions[cid] = currentBet;
                    break;

                case ActionType.Raise:
                    table.Pots.AddBet(m, amt - m.CurrentBet);
                    currentBet = amt;
                    contributions[cid] = amt;
                    break;

                case ActionType.Check:
                    break;
            }

            if (m.Stack == 0) allIn.Add(cid);
        }

        /* ------------- Завершение улицы ------------ */

        bool CheckStreetEnd()
        {
            // остался 1 игрок
            if (turnOrder.Count(id => !folded.Contains(id)) <= 1) { EndStreet(); return true; }

            // все уравняли ставки
            foreach (var cid in turnOrder)
                if (!folded.Contains(cid) && !allIn.Contains(cid) &&
                    (!contributions.TryGetValue(cid, out int v) || v < currentBet))
                    return false;

            EndStreet();
            return true;
        }

        void EndStreet()
        {
            bettingActive = false;
            Debug.Log($"[Betting] {currentStreet} done. Pot={table.Pots.Total}");

            if (currentStreet == Street.River)
            {
                table.FinishHandShowdown();       // ← вызов шоудауна
                currentStreet = Street.Showdown;
            }
            else
            {
                currentStreet = NextStreet(currentStreet);
                BeginStreet();
            }
        }

        Street NextStreet(Street s) => s switch
        {
            Street.PreFlop => Street.Flop,
            Street.Flop    => Street.Turn,
            Street.Turn    => Street.River,
            _              => Street.Showdown
        };

        /* ------------- helpers ------------- */

        PokerPlayerModel FindModel(ulong cid) =>
            FindObjectsByType<PokerPlayerController>(FindObjectsSortMode.None)
                .FirstOrDefault(p => p.GetComponent<NetworkObject>()?.OwnerClientId == cid)?.Model;

        IPlayerGateway FindGateway(ulong cid) =>
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .FirstOrDefault(x => x is IPlayerGateway gw && gw.Seat == (int)cid) as IPlayerGateway;

        ulong? GetClientIdBySeat(int seat) =>
            FindObjectsByType<PokerPlayerController>(FindObjectsSortMode.None)
                .FirstOrDefault(p => p.Model.Id == seat)?
                .GetComponent<NetworkObject>()?.OwnerClientId;
    }
}
