using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Poker.Game.Betting;

namespace Poker.GameLoop
{
    /// <summary>FSM-движок: Preflop → Flop → Turn → River → Showdown.</summary>
    public sealed class GameStateManager : MonoBehaviour
    {
        [SerializeField] private RoundManager  round;
        [SerializeField] private BettingSystem betting;

        public event System.Action RoundStarted; // 👈 новое событие

        readonly Queue<IGameState> sequence = new();

        void Start()
        {
            EnqueueStandardSequence();
            StartCoroutine(StateMachine());
        }

        void EnqueueStandardSequence()
        {
            sequence.Enqueue(new PreflopState(this));
            sequence.Enqueue(new FlopState(this));
            sequence.Enqueue(new TurnState(this));
            sequence.Enqueue(new RiverState(this));
            sequence.Enqueue(new ShowdownState(this));
        }

        IEnumerator StateMachine()
        {
            while (true)
            {
                if (sequence.Count == 0) EnqueueStandardSequence();
                yield return sequence.Dequeue().Execute();
            }
        }

        #region Accessors

        public RoundManager  Round   => round;
        public BettingSystem Betting => betting;

        public List<PokerPlayerController> ActivePlayers() =>
            Object.FindObjectsByType<PokerPlayerController>(FindObjectsSortMode.None).ToList();

        #endregion

        #region concrete states ---------------------------------------------

        sealed class PreflopState : IGameState
        {
            readonly GameStateManager ctx;
            public PreflopState(GameStateManager ctx) => this.ctx = ctx;

            public IEnumerator Execute()
            {
                yield return ctx.Round.SetupNewHandRoutine();
                ctx.RoundStarted?.Invoke(); // 👈 уведомляем сервер о начале раздачи

                yield return ctx.Round.DealHoleCardsRoutine();
                yield return ctx.Betting.ExecuteBettingRound(ctx.ActivePlayers(), Street.Preflop);
            }
        }

        sealed class FlopState : IGameState
        {
            readonly GameStateManager ctx;
            public FlopState(GameStateManager ctx) => this.ctx = ctx;

            public IEnumerator Execute()
            {
                yield return ctx.Round.RevealFlopRoutine();
                yield return ctx.Betting.ExecuteBettingRound(ctx.ActivePlayers(), Street.Flop);
            }
        }

        sealed class TurnState : IGameState
        {
            readonly GameStateManager ctx;
            public TurnState(GameStateManager ctx) => this.ctx = ctx;

            public IEnumerator Execute()
            {
                yield return ctx.Round.RevealTurnRoutine();
                yield return ctx.Betting.ExecuteBettingRound(ctx.ActivePlayers(), Street.Turn);
            }
        }

        sealed class RiverState : IGameState
        {
            readonly GameStateManager ctx;
            public RiverState(GameStateManager ctx) => this.ctx = ctx;

            public IEnumerator Execute()
            {
                yield return ctx.Round.RevealRiverRoutine();
                yield return ctx.Betting.ExecuteBettingRound(ctx.ActivePlayers(), Street.River);
            }
        }

        sealed class ShowdownState : IGameState
        {
            readonly GameStateManager ctx;
            public ShowdownState(GameStateManager ctx) => this.ctx = ctx;

            public IEnumerator Execute()
            {
                ctx.Round.EvaluateWinners();
                yield return new WaitForSeconds(3f);
            }
        }

        #endregion
    }
}
