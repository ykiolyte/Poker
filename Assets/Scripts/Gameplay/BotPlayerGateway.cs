using System;
using Poker.Gameplay;

namespace Poker.Gameplay
{
    public class BotPlayerGateway : IPlayerGateway
    {
        private readonly SimpleBotDecisionMaker _bot;
        private readonly IHandRankingService    _handService;
        private int _stack;

        public int  Seat       { get; }
        public int  CurrentBet { get; private set; }
        public bool IsInHand   { get; private set; } = true;

        public BotPlayerGateway(
            int seat,
            SimpleBotDecisionMaker bot,
            IHandRankingService handService,
            int initialStack)
        {
            Seat         = seat;
            _bot         = bot;
            _handService = handService;
            _stack       = initialStack;
        }

        public void RequestAction(PlayerContext ctx, Action<PlayerAction> cb)
        {
            var strength = _handService.EvaluateProbabilistic(Seat);
            cb(_bot.Decide(ctx, strength, _stack));
        }

        public void Pay(int amount) { _stack -= amount; CurrentBet += amount; }
        public void Fold()          => IsInHand = false;
    }
}
