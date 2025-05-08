using UnityEngine;

public class PreflopState : IGameState
{
    private readonly GameStateManager manager;

    public PreflopState(GameStateManager manager)
    {
        this.manager = manager;
    }

    public void Enter()
    {
        LoggerService.Log("Entering Preflop state", LogLevel.Info);
        // TODO: раздача карт игрокам, установка блайндов
    }

    public void Exit()
    {
        LoggerService.Log("Exiting Preflop state", LogLevel.Info);
    }

    public void Update()
    {
        // TODO: Обработка действий игроков, переход к следующей фазе, если все завершили ход
    }
}
