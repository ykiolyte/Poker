using System;
using UnityEngine;

namespace Poker.Gameplay
{
    /// <summary>Scriptable-мьютекс для критических сетевых секций.</summary>
    [CreateAssetMenu(fileName = "NetworkLock", menuName = "Poker/Network/NetworkLock")]
    public class NetworkLock : ScriptableObject
    {
        private readonly object _syncRoot = new();

        /// <summary>Выполняет <paramref name="action"/> под lock-ом.</summary>
        public void Execute(Action action)
        {
            if (action == null) return;
            lock (_syncRoot) { action(); }
        }
    }
}
