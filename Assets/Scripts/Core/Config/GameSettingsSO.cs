using UnityEngine;

namespace Poker.Core.Config
{
    /// <summary>
    /// Глобальные настройки стола и ставок.
    /// Редактируется из Инспектора, читается только через публичные свойства.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Poker/Config/Game Settings")]
    public class GameSettingsSO : ScriptableObject
    {
        [Header("Blinds")]
        [SerializeField] private int bigBlind = 100;
        [SerializeField] private int smallBlind = 50;

        [Header("Gameplay")]
        [SerializeField] private int startingChips = 2000;
        [SerializeField, Range(2, 9)] private int maxPlayers = 6;

        // Свойства‑только‑чтение, исключают прямую запись извне.
        public int BigBlind   => bigBlind;
        public int SmallBlind => smallBlind;
        public int StartingChips => startingChips;
        public int MaxPlayers => maxPlayers;
    }
}
