using System;

namespace Poker.Gameplay
{
    public interface IPlayerGateway
    {
        int  Seat        { get; }
        int  CurrentBet  { get; }
        bool IsInHand    { get; }

        void RequestAction(PlayerContext ctx, Action<PlayerAction> callback);
        void Pay(int amount);
        void Fold();
    }

    public readonly struct PlayerContext
    {
        public readonly int    MaxBet;
        public readonly Street Street;
        public PlayerContext(int maxBet, Street street) { MaxBet = maxBet; Street = street; }
    }

    public readonly struct PlayerAction
    {
        public readonly ActionType Type;
        public readonly int        Amount;

        private PlayerAction(ActionType type, int amount = 0) { Type = type; Amount = amount; }

        public static PlayerAction Fold()                => new(ActionType.Fold);
        public static PlayerAction Check()               => new(ActionType.Check);
        public static PlayerAction Call()                => new(ActionType.Call);
        public static PlayerAction Raise(int amount)     => new(ActionType.Raise, amount);
    }

    public enum ActionType { Fold, Check, Call, Raise }
}
