using System;
using Unity.Netcode;
using UnityEngine;
using Poker.Gameplay;
using Poker.Game.Betting;
using Poker.Presentation;            // ← UI-адаптер
using Poker.Server;                  // ← NetworkBettingEngine

namespace Poker.Gameplay
{
    public sealed class RemotePlayerGateway : NetworkBehaviour, IPlayerGateway
    {
        [SerializeField] private int  seat;
        [SerializeField] private int  currentBet;
        [SerializeField] private bool isInHand = true;

        public int  Seat       => seat;
        public int  CurrentBet => currentBet;
        public bool IsInHand   => isInHand;

        /* ---------- IPlayerGateway ---------- */

        public void Initialise(int seat)
        {
            this.seat  = seat;
            currentBet = 0;
            isInHand   = true;
        }

        public void RequestAction(PlayerContext ctx, Action<PlayerAction> cb)
        {
            var ui = FindFirstObjectByType<PlayerUIAdapter>();
            if (ui == null)
            {
                Debug.LogError("[RemoteGW] UI adapter not found");
                return;
            }
            ui.QueryAction(ctx, cb);
        }

        public void Pay (int amt) => currentBet += amt;
        public void Fold()        => isInHand   = false;

        /* ---------- RPC ---------- */

        [ServerRpc(RequireOwnership = false)]
        public void SubmitActionServerRpc(int type, int amount)
        {
            if (!IsServer) return;

            var engine = FindFirstObjectByType<NetworkBettingEngine>();
            engine?.ReceiveRemoteAction(seat, (ActionType)type, amount);
        }

        [ClientRpc]
        public void ActionAppliedClientRpc(int seat, int type, int amount)
        {
            if (seat != this.seat) return;

            switch ((ActionType)type)
            {
                case ActionType.Fold:  Fold();                      break;
                case ActionType.Call:  Pay(amount);                 break;
                case ActionType.Raise: Pay(amount - currentBet);    break;
                case ActionType.Check:                               break;
            }
        }
    }
}
