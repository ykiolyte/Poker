// Assets/Scripts/Core/Config/GameSettingsSO.cs
using UnityEngine;

namespace Poker.Core.Config
{
    /// <summary>Настройка стола (блайнды, стеки, лимиты и т.д.).</summary>
    //  ❗ Убрали sealed, чтобы тесты могли создать подкласс
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Poker/Config/Game Settings")]
    public class GameSettingsSO : ScriptableObject
    {
        [Header("Blinds / Antes")]
        [Min(1)]  [SerializeField] private int smallBlind   = 10;
        [Min(2)]  [SerializeField] private int bigBlind     = 20;

        [Header("Stacks / Buyins")]
        [Min(1)]  [SerializeField] private int startingChips = 1000;
        [Min(1)]  [SerializeField] private int maxBuyIn      = 2000;

        [Header("Players")]
        [Range(2, 10)] [SerializeField] private int maxPlayers = 6;

        // --- read‑only API ---
        public int SmallBlind    => smallBlind;
        public int BigBlind      => bigBlind;
        public int StartingChips => startingChips;
        public int MaxBuyIn      => maxBuyIn;
        public int MaxPlayers    => maxPlayers;

        /// <summary>Пустой метод, который вызывают юнит‑тесты.</summary>
        public virtual void Init
        (
            int sb = 10,
            int bb = 20,
            int stack = 1000,
            int maxBuy = 2000,
            int players = 6
        )
        {
            smallBlind    = sb;
            bigBlind      = bb;
            startingChips = stack;
            maxBuyIn      = maxBuy;
            maxPlayers    = players;
        }
    }
}
