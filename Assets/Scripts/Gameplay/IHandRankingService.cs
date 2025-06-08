namespace Poker.Gameplay
{
    /// <summary>Возвращает нормированную силу руки (0–1).</summary>
    public interface IHandRankingService
    {
        HandStrength EvaluateProbabilistic(int seat);
    }
}
