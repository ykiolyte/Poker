using UnityEngine;

namespace Poker.Gameplay.Cards
{
    /// <summary>
    /// Отдаёт позиции для карт в правой/левой руке игрока.
    /// </summary>
    public sealed class CardAnchorProvider : MonoBehaviour
    {
        [SerializeField] private Transform rightHandAnchor;
        [SerializeField] private Transform leftHandAnchor;

        public Transform GetAnchor(bool isLeftHand = false)
            => isLeftHand ? leftHandAnchor : rightHandAnchor;
    }
}
