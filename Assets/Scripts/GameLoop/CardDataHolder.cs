// Assets/Scripts/GameLoop/CardDataHolder.cs
using UnityEngine;
using Poker.Gameplay.Cards;

public sealed class CardDataHolder : MonoBehaviour
{
    public CardDataSO DataSO { get; private set; }

    public void Init(CardDataSO data) => DataSO = data;
}
