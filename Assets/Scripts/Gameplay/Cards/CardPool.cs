using Poker.Infrastructure.Pooling;
using Poker.UI;                 // ⬅ добавили
using UnityEngine;

namespace Poker.Gameplay.Cards
{
    public sealed class CardPool : MonoBehaviour
    {
        [SerializeField] private GenericObjectPool<CardView> pool;

        public CardView  GetCard()          => pool.Spawn();
        public void      ReturnCard(CardView card) => pool.Despawn(card);
    }
}
