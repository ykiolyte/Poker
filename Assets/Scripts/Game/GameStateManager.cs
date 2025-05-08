using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private readonly Dictionary<GamePhase, IGameState> states = new();
    private IGameState currentState;

    [SerializeField] private GamePhase startingPhase = GamePhase.Preflop;

    private void Awake()
    {
        states[GamePhase.Preflop] = new PreflopState(this);
        states[GamePhase.Flop] = new FlopState(this);
        states[GamePhase.Turn] = new TurnState(this);
        states[GamePhase.River] = new RiverState(this);
        states[GamePhase.Showdown] = new ShowdownState(this);
    }

    private void Start()
    {
        ChangeState(startingPhase);
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(GamePhase nextPhase)
    {
        currentState?.Exit();
        currentState = states[nextPhase];
        currentState.Enter();
        LoggerService.Log($"Game state changed to: {nextPhase}", LogLevel.Info);
    }
}
