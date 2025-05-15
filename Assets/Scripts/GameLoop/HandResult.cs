using System.Collections.Generic;

namespace Poker.GameLoop
{
    /// <summary>
    /// Результат оценки 7-карточной руки:
    /// Score (ранг + кикеры) и сами 5 карточек выигрышной пятёрки.
    /// </summary>
    public sealed class HandResult
    {
        public HandScore Score { get; }
        public List<CardDataSO> Combination { get; }

        public HandResult(HandScore score, List<CardDataSO> combination)
        {
            Score = score;
            Combination = combination;
        }
    }
}
