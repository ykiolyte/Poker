using UnityEngine;

namespace Poker.Core.Data
{
    [System.Serializable]
    public struct CardData
    {
        public Suit suit;
        public Rank rank;
        public Sprite sprite;
    }

    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Two = 2,
        Three, Four, Five, Six, Seven, Eight, Nine, Ten,
        Jack, Queen, King, Ace
    }
}
