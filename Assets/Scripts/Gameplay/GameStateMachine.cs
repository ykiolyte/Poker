namespace Poker.Gameplay
{
    public enum Street { PreFlop, Flop, Turn, River, Showdown }

    public class GameStateMachine
    {
        public Street CurrentStreet { get; private set; } = Street.PreFlop;
        public bool   IsShowdown    => CurrentStreet == Street.Showdown;

        public void GoToNextStreet()
        {
            CurrentStreet = CurrentStreet switch
            {
                Street.PreFlop => Street.Flop,
                Street.Flop    => Street.Turn,
                Street.Turn    => Street.River,
                _              => Street.Showdown
            };
        }
    }
}
