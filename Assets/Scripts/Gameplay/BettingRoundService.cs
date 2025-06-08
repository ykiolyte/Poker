using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Poker.Gameplay
{
    public class BettingRoundService
    {
        private readonly List<IPlayerGateway> _gateways;
        private readonly GameStateMachine     _stateMachine;
        private readonly NetworkTimer         _timer;
        private readonly NetworkLock          _lock;

        private int  _dealerIndex;
        private int  _currentPlayer;
        private int  _maxBet;
        private bool _hasRaise;

        public event Action<int, PlayerAction> OnPlayerActed;
        public event Action<Street>            OnStreetEnded;

        public BettingRoundService(
            List<IPlayerGateway> gateways,
            GameStateMachine stateMachine,
            NetworkTimer timer,
            NetworkLock networkLock,
            int dealerIndex = 0)
        {
            _gateways     = gateways;
            _stateMachine = stateMachine;
            _timer        = timer;
            _lock         = networkLock;
            _dealerIndex  = dealerIndex;
            _currentPlayer = NextPlayer(dealerIndex + 1);
        }

        public void StartRound()
        {
            _maxBet   = _stateMachine.CurrentStreet == Street.PreFlop ? SmallBlind : 0;
            _hasRaise = false;
            RequestAction();
        }

        private void RequestAction()
        {
            var gateway = _gateways[_currentPlayer];
            _timer.Begin(PlayerTimeout, () => ForceFold(gateway));
            gateway.RequestAction(
                new PlayerContext(_maxBet, _stateMachine.CurrentStreet),
                action => ResolveAction(gateway, action));
        }

        private void ResolveAction(IPlayerGateway gateway, PlayerAction action)
        {
            _timer.Cancel();
            _lock.Execute(() =>
            {
                switch (action.Type)
                {
                    case ActionType.Fold:
                        gateway.Fold();
                        break;
                    case ActionType.Check:
                        break;
                    case ActionType.Call:
                        gateway.Pay(_maxBet - gateway.CurrentBet);
                        break;
                    case ActionType.Raise:
                        gateway.Pay(action.Amount - gateway.CurrentBet);
                        _maxBet   = action.Amount;
                        _hasRaise = true;
                        break;
                }

                OnPlayerActed?.Invoke(gateway.Seat, action);
                AdvanceTurn();
            });
        }

        private void AdvanceTurn()
        {
            _currentPlayer = NextPlayer(_currentPlayer + 1);

            if (IsStreetComplete())
            {
                OnStreetEnded?.Invoke(_stateMachine.CurrentStreet);
                _stateMachine.GoToNextStreet();
                if (_stateMachine.IsShowdown) return;
                StartRound();
            }
            else
            {
                RequestAction();
            }
        }

        private bool IsStreetComplete()
            => EveryoneFoldedButOne() ||
               (_maxBet == AllPlayersCurrentBet() &&
               (_hasRaise || _stateMachine.CurrentStreet == Street.PreFlop));

        private bool EveryoneFoldedButOne()
        {
            int active = 0;
            foreach (var g in _gateways) if (g.IsInHand) active++;
            return active <= 1;
        }

        private int AllPlayersCurrentBet()
        {
            int min = int.MaxValue;
            foreach (var g in _gateways) if (g.IsInHand) min = Math.Min(min, g.CurrentBet);
            return min;
        }

        private int NextPlayer(int idx)
        {
            int attempts = 0;
            while (attempts++ < _gateways.Count)
            {
                int i = (idx + attempts) % _gateways.Count;
                if (_gateways[i].IsInHand) return i;
            }
            return -1;
        }

        private void ForceFold(IPlayerGateway gateway)
            => ResolveAction(gateway, PlayerAction.Fold());

        private const int   SmallBlind    = 50;
        private const float PlayerTimeout = 25f; // seconds
    }
}
