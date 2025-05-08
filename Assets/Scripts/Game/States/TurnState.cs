public class TurnState : IGameState
{
    private readonly GameStateManager manager;

    public TurnState(GameStateManager manager)
    {
        this.manager = manager;
    }

    public void Enter()
    {
        LoggerService.Log("Entering Turn state", LogLevel.Info);
        // TODO: reveal 4th community card
    }

    public void Exit()
    {
        LoggerService.Log("Exiting Turn state", LogLevel.Info);
    }

    public void Update()
    {
        // TODO: transition after betting
    }
}