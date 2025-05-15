using System.Collections.Generic;
using UnityEngine;
using Poker.Core.Data;

namespace Poker.Core.Config
{
    [CreateAssetMenu(fileName = "DeckConfig", menuName = "Poker/Config/Deck")]
    public sealed class DeckConfigSO : ScriptableObject
    {
        [SerializeField] private List<CardData> cards = new();

        public Sprite GetSprite(Rank rank, Suit suit)
        {
            foreach (var card in cards)
            {
                if (card.rank == rank && card.suit == suit)
                    return card.sprite;
            }

            Debug.LogWarning($"[DeckConfigSO] No sprite found for {rank} of {suit}");
            return null;
        }

        public IReadOnlyList<CardData> Cards => cards;
    }
}
