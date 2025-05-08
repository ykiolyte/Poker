public class RiverState : IGameState
{
    private readonly GameStateManager manager;

    public RiverState(GameStateManager manager)
    {
        this.manager = manager;
    }

    public void Enter()
    {
        LoggerService.Log("Entering River state", LogLevel.Info);
        // TODO: reveal final card
    }

    public void Exit()
    {
        LoggerService.Log("Exiting River state", LogLevel.Info);
    }

    public void Update()
    {
        // TODO: transition to showdown
    }
}