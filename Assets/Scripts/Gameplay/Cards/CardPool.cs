using UnityEngine;

namespace Poker.Gameplay.Cards
{
    public sealed class CardPool : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour poolRef;

        public CardView3D Spawn()
        {
            var method = poolRef.GetType().GetMethod("Spawn");
            return method?.Invoke(poolRef, null) as CardView3D;
        }

        public void Despawn(CardView3D card)
        {
            var method = poolRef.GetType().GetMethod("Despawn");
            method?.Invoke(poolRef, new object[] { card });
        }
    }
}
