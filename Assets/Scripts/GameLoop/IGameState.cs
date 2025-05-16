using System.Collections;

namespace Poker.GameLoop
{
    public interface IGameState
    {
        IEnumerator Execute();
    }
}
