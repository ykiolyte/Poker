public class FlopState : IGameState
{
    private readonly GameStateManager manager;

    public FlopState(GameStateManager manager)
    {
        this.manager = manager;
    }

    public void Enter()
    {
        LoggerService.Log("Entering Flop state", LogLevel.Info);
        // TODO: reveal 3 community cards
    }

    public void Exit()
    {
        LoggerService.Log("Exiting Flop state", LogLevel.Info);
    }

    public void Update()
    {
        // TODO: next phase if betting round complete
    }
}