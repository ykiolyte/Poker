using Poker.Gameplay;
using Unity.Netcode;
using UnityEngine;

namespace Poker.Gameplay
{
    [RequireComponent(typeof(NetworkObject))]
    public class RemotePlayerGatewayStub : NetworkBehaviour, IPlayerGateway
    {
        public int  Seat       { get; private set; }
        public int  CurrentBet { get; private set; }
        public bool IsInHand   { get; private set; } = true;

        public void Initialise(int seat) => Seat = seat;

        public void RequestAction(PlayerContext ctx, System.Action<PlayerAction> cb)
        {
            // TODO: заменить на RPC к клиенту
            cb(PlayerAction.Fold()); // failsafe
        }

        public void Pay(int amount) { CurrentBet += amount; }
        public void Fold()          => IsInHand = false;
    }
}
