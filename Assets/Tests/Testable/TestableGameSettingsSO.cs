using UnityEngine;
using Poker.Core.Config;

#if UNITY_EDITOR
namespace Tests.Testables
{
    /// <summary>
    /// Тестовая реализация GameSettingsSO для юнит‑тестов.
    /// Позволяет задавать значения из тестов, сохраняя инкапсуляцию.
    /// </summary>
    public sealed class TestableGameSettingsSO : GameSettingsSO
    {
        public void Init(int small, int big, int chips, int max)
        {
            SetFieldsForTesting(small, big, chips, max);
        }

        private void SetFieldsForTesting(int small, int big, int chips, int max)
        {
            this.GetType().BaseType
                .GetField("smallBlind", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(this, small);

            this.GetType().BaseType
                .GetField("bigBlind", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(this, big);

            this.GetType().BaseType
                .GetField("startingChips", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(this, chips);

            this.GetType().BaseType
                .GetField("maxPlayers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(this, max);
        }
    }
}
#endif
