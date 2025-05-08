using System.Collections.Generic;

public enum HandRank
{
    HighCard = 1,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush
}

public class HandValue
{
    public HandRank Rank;
    public List<CardRank> Kickers = new();
}
