using System.Collections.Generic;

public class GameStateMachine
{
    private IGameState currentState;
    private readonly Dictionary<GamePhase, IGameState> states = new();

    public void RegisterState(GamePhase phase, IGameState state)
    {
        states[phase] = state;
    }

    public void ChangeState(GamePhase newPhase)
    {
        currentState?.Exit();
        currentState = states[newPhase];
        currentState.Enter();
    }

    public void Tick() => currentState?.Update();
}
