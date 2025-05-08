public class ShowdownState : IGameState
{
    private readonly GameStateManager manager;

    public ShowdownState(GameStateManager manager)
    {
        this.manager = manager;
    }

    public void Enter()
    {
        LoggerService.Log("Entering Showdown state", LogLevel.Info);
        // TODO: reveal hands, evaluate winner
    }

    public void Exit()
    {
        LoggerService.Log("Exiting Showdown state", LogLevel.Info);
    }

    public void Update()
    {
        // TODO: wait, reset hand
    }
}
