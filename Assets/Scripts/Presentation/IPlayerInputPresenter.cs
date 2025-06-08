using System;
using Poker.Gameplay;

namespace Poker.Presentation
{
    public interface IPlayerInputPresenter
    {
        void QueryAction(PlayerContext context, Action<PlayerAction> callback);
    }
}
