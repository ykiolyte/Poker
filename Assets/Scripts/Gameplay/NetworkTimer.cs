using System;
using Unity.Netcode;
using UnityEngine;

namespace Poker.Gameplay
{
    public class NetworkTimer : NetworkBehaviour
    {
        private readonly NetworkVariable<float> _timeLeft =
            new(0f, NetworkVariableReadPermission.Everyone);

        private Action _onTimeout;
        private bool   _running;

        public void Begin(float seconds, Action onTimeout)
        {
            if (!IsServer) return;
            _timeLeft.Value = seconds;
            _onTimeout      = onTimeout;
            _running        = true;
        }

        public void Cancel()
        {
            if (!IsServer) return;
            _running = false;
        }

        private void Update()
        {
            if (!IsServer || !_running) return;
            _timeLeft.Value -= Time.deltaTime;
            if (_timeLeft.Value <= 0f)
            {
                _running = false;
                _onTimeout?.Invoke();
            }
        }
    }
}
