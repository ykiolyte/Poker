using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Configs/Card Data")]
public class CardDataSO : ScriptableObject
{
    public CardSuit suit;
    public CardRank rank;
    public Sprite cardFront;
    public Sprite cardBack;
}

public enum CardSuit { Clubs, Diamonds, Hearts, Spades }
public enum CardRank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }
