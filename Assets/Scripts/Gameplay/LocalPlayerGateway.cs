using System;
using Poker.Gameplay;
using Poker.Presentation;

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
            => _presenter.QueryAction(ctx, action => cb(action));

        public void Pay(int amount) { CurrentBet += amount; }
        public void Fold()          => IsInHand = false;
    }
}
