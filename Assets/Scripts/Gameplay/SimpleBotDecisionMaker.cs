using System;
using UnityEngine;

namespace Poker.Gameplay
{
    public class SimpleBotDecisionMaker
    {
        public enum Aggression { Passive, Neutral, Aggressive }
        private readonly Aggression _level;
        public SimpleBotDecisionMaker(Aggression level) => _level = level;

        public PlayerAction Decide(PlayerContext ctx, HandStrength hand, int stack)
        {
            float score = hand.Normalized * (1f + (int)_level * 0.2f);
            if (score < 0.3f) return PlayerAction.Fold();
            if (score < 0.6f) return ctx.MaxBet == 0 ? PlayerAction.Check() : PlayerAction.Call();

            int raise = Mathf.Min(stack, ctx.MaxBet + Mathf.RoundToInt(stack * score * 0.2f));
            return PlayerAction.Raise(raise);
        }
    }

    public readonly struct HandStrength
    {
        public readonly float Normalized;
        public HandStrength(float n) => Normalized = Mathf.Clamp01(n);
    }
}
