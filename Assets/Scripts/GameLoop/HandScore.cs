using System;

namespace Poker.GameLoop
{
    /// <summary>
    /// Ранг руки + вспомогательные кикеры, для сравнения.
    /// </summary>
    public sealed class HandScore : IComparable<HandScore>
    {
        public HandRank Rank { get; }
        public int[] Kickers { get; }

        public HandScore(HandRank rank, int[] kickers)
        {
            Rank = rank;
            Kickers = kickers;
        }

        public int CompareTo(HandScore other)
        {
            if (Rank != other.Rank)
                return Rank.CompareTo(other.Rank);
            // сравниваем кикеры по порядку
            for (int i = 0; i < Kickers.Length && i < other.Kickers.Length; i++)
            {
                if (Kickers[i] != other.Kickers[i])
                    return Kickers[i].CompareTo(other.Kickers[i]);
            }
            return 0;
        }
    }
}
