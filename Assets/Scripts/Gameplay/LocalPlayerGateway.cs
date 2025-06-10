using System;
using Poker.Gameplay;
using Poker.Presentation;
using Unity.Netcode;
using Poker.Server; // если NetworkBettingEngine лежит в этом namespace


namespace Poker.Gameplay
{
    public class LocalPlayerGateway : IPlayerGateway
    {
        private readonly IPlayerInputPresenter _presenter;

        public int  Seat       { get; }
        public int  CurrentBet { get; private set; }
        public bool IsInHand   { get; private set; } = true;

        public LocalPlayerGateway(int seat, IPlayerInputPresenter presenter)
        { Seat = seat; _presenter = presenter; }

         public void RequestAction(PlayerContext ctx, Action<PlayerAction> cb)
        {
            _presenter.QueryAction(ctx, action =>
            {
                NetworkManager.Singleton.LocalClient.PlayerObject
                    .GetComponent<NetworkBettingEngine>()
                    .SubmitActionServerRpc(action.Type, action.Amount);
                cb(action); // локально сообщаем presenter-у, но модель меняет только сервер
            });
        }

        public void Pay(int amount) { CurrentBet += amount; }
        public void Fold()          => IsInHand = false;
    }
}
