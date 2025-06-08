using Poker.Gameplay;
using UnityEngine;

namespace Poker.Gameplay
{
    /// <summary>Временная заглушка для оценки силы руки (рандом 0–1).</summary>
    public class DummyHandRanking : IHandRankingService
    {
        public HandStrength EvaluateProbabilistic(int seat)
        {
            return new HandStrength(Random.value);
        }
    }
}
