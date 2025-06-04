// Assets/Scripts/Infrastructure/Timers/BettingTimerService.cs
using System;
using UnityEngine;

namespace Poker.Infrastructure.Timers
{
    /// <summary>
    /// Сервис таймера хода игрока. Передаёт оставшееся время через событие Tick.
    /// </summary>
    public sealed class BettingTimerService : MonoBehaviour
    {
        /// <summary>Вызывается каждый кадр: t ∈ [1..0]</summary>
        public event Action<float> TimerTick;

        /// <summary>Вызывается один раз, когда время истекло.</summary>
        public event Action TimerExpired;

        private float duration;
        private float elapsed;
        private bool isRunning;

        /// <summary>Запуск таймера (секунд).</summary>
        public void StartTimer(float seconds)
        {
            duration = Mathf.Max(seconds, 0.1f);
            elapsed = 0f;
            isRunning = true;
        }

        /// <summary>Принудительная остановка таймера.</summary>
        public void StopTimer()
        {
            isRunning = false;
        }

        private void Update()
        {
            if (!isRunning) return;

            elapsed += Time.deltaTime;
            float t01 = 1f - Mathf.Clamp01(elapsed / duration);
            TimerTick?.Invoke(t01);

            if (elapsed >= duration)
            {
                isRunning = false;
                TimerExpired?.Invoke();
            }
        }
    }
}
