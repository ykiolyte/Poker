using System.Collections.Generic;
using System.Linq;
using Poker.GameLoop;
using Poker.Domain.Betting;

namespace Poker.Server
{
    public static class ShowdownService
    {
        public static Dictionary<int, int> Evaluate(
            Dictionary<int, int> contributions,
            Dictionary<int, HandResult> hands)
        {
            var payout = new Dictionary<int, int>();
            var pots = SidePotManager.Build(contributions);

            foreach (var (amount, contenders) in pots)
            {
                var bestScore = contenders
                    .Select(id => hands[id].Score)
                    .Aggregate((a, b) => a.CompareTo(b) > 0 ? a : b);

                var winners = contenders
                    .Where(id => hands[id].Score.CompareTo(bestScore) == 0)
                    .ToList();

                int share = amount / winners.Count;
                foreach (int w in winners)
                    payout[w] = payout.GetValueOrDefault(w) + share;
            }

            return payout;
        }
    }
}