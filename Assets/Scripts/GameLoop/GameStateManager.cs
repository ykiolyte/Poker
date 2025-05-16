using System.Collections;
using System.Collections.Generic;
using System.Linq;            // ← для ToList()
using UnityEngine;

namespace Poker.GameLoop
{
    /// <summary>
    /// FSM-движок: Preflop → Flop → Turn → River → Showdown → новая раздача.
    /// </summary>
    public sealed class GameStateManager : MonoBehaviour
    {
        [SerializeField] private RoundManager  round;
        [SerializeField] private BettingSystem betting;

        private readonly Queue<IGameState> sequence = new();

        private void Start()
        {
            EnqueueStandardSequence();
            StartCoroutine(StateMachine());
        }

        private void EnqueueStandardSequence()
        {
            sequence.Enqueue(new PreflopState(this));
            sequence.Enqueue(new FlopState(this));
            sequence.Enqueue(new TurnState(this));
            sequence.Enqueue(new RiverState(this));
            sequence.Enqueue(new ShowdownState(this));
        }

        private IEnumerator StateMachine()
        {
            while (true)
            {
                if (sequence.Count == 0) EnqueueStandardSequence();

                var state = sequence.Dequeue();
                yield return state.Execute();
            }
        }

        #region accessors ----------------------------------------------------

        public RoundManager  Round   => round;
        public BettingSystem Betting => betting;

        #endregion
    }

    #region concrete states --------------------------------------------------

    sealed class PreflopState : IGameState
    {
        private readonly GameStateManager ctx;
        public PreflopState(GameStateManager ctx) => this.ctx = ctx;

        public IEnumerator Execute()
        {
            yield return ctx.Round.SetupNewHandRoutine();
            yield return ctx.Round.DealHoleCardsRoutine();
            yield return ctx.Betting.ExecuteBettingRound(
                ctx.Round.GetComponentsInChildren<PokerPlayerController>().ToList());
        }
    }

    sealed class FlopState : IGameState
    {
        private readonly GameStateManager ctx;
        public FlopState(GameStateManager ctx) => this.ctx = ctx;

        public IEnumerator Execute()
        {
            yield return ctx.Round.RevealFlopRoutine();
            yield return ctx.Betting.ExecuteBettingRound(
                ctx.Round.GetComponentsInChildren<PokerPlayerController>().ToList());
        }
    }

    sealed class TurnState : IGameState
    {
        private readonly GameStateManager ctx;
        public TurnState(GameStateManager ctx) => this.ctx = ctx;

        public IEnumerator Execute()
        {
            yield return ctx.Round.RevealTurnRoutine();
            yield return ctx.Betting.ExecuteBettingRound(
                ctx.Round.GetComponentsInChildren<PokerPlayerController>().ToList());
        }
    }

    sealed class RiverState : IGameState
    {
        private readonly GameStateManager ctx;
        public RiverState(GameStateManager ctx) => this.ctx = ctx;

        public IEnumerator Execute()
        {
            yield return ctx.Round.RevealRiverRoutine();
            yield return ctx.Betting.ExecuteBettingRound(
                ctx.Round.GetComponentsInChildren<PokerPlayerController>().ToList());
        }
    }

    sealed class ShowdownState : IGameState
    {
        private readonly GameStateManager ctx;
        public ShowdownState(GameStateManager ctx) => this.ctx = ctx;

        public IEnumerator Execute()
        {
            ctx.Round.EvaluateWinners();
            yield return new WaitForSeconds(3f); // пауза перед новой раздачей
        }
    }

    #endregion
}
