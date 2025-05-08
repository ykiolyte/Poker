using System;
using System.Collections.Generic;
using System.Linq;

public static class WinnerDeterminer
{
    public static Dictionary<PokerPlayerModel, int> DistributeSidePots(List<PokerPlayerModel> players, List<CardDataSO> communityCards, int totalPot)
    {
        var result = new Dictionary<PokerPlayerModel, int>();
        var remainingPlayers = players.Where(p => !p.IsFolded).OrderBy(p => p.TotalBet).ToList();
        int lastCap = 0;
        var sidePotPlayers = new List<PokerPlayerModel>(players);

        foreach (var cap in remainingPlayers.Select(p => p.TotalBet).Distinct())
        {
            int level = cap - lastCap;
            int levelPot = 0;
            var eligible = new List<PokerPlayerModel>();

            foreach (var p in sidePotPlayers)
            {
                int bet = Math.Min(level, p.TotalBet - lastCap);
                if (bet > 0)
                {
                    levelPot += bet;
                    if (!p.IsFolded)
                        eligible.Add(p);
                }
            }

            var evaluated = eligible.ToDictionary(
                p => p,
                p => HandEvaluator.EvaluateHand(p.HoleCards, communityCards));
            var best = evaluated.Values.OrderByDescending(v => v, Comparer<HandValue>.Create(HandEvaluator.CompareHands)).First();
            var winners = evaluated.Where(kvp => HandEvaluator.CompareHands(kvp.Value, best) == 0).Select(kvp => kvp.Key).ToList();

            int share = levelPot / winners.Count;
            foreach (var w in winners)
                result[w] = result.GetValueOrDefault(w, 0) + share;

            lastCap = cap;
        }

        return result;
    }
}
