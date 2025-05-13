using UnityEngine;
using Poker.Infrastructure.Pooling;

namespace Poker.UI
{
    /// <summary>
    /// Визуальное представление одной карты на столе.
    /// Умеет подставлять спрайт и участвует в пуле.
    /// </summary>
    public sealed class CardView : MonoBehaviour, IPoolable
    {
        [Header("Renderers")]
        [SerializeField] private SpriteRenderer faceRenderer;
        [SerializeField] private SpriteRenderer backRenderer;

        /// <summary>
        /// Устанавливает спрайт карты и показывает/прячет рубашку.
        /// </summary>
        public void SetSprite(Sprite faceSprite, bool showBack = false)
        {
            faceRenderer.sprite = faceSprite;
            backRenderer.enabled = showBack;
            faceRenderer.enabled = !showBack;
        }

        /// <summary>
        /// Простая псевдо-анимация переворота карты.
        /// </summary>
        public void Flip()
        {
            bool showingBack = backRenderer.enabled;
            backRenderer.enabled = !showingBack;
            faceRenderer.enabled = showingBack;
        }

        public void OnSpawned()
        {
            faceRenderer.sprite = null;
            backRenderer.enabled = false;
            faceRenderer.enabled = false;
            transform.localScale = Vector3.one;
        }

        public void OnDespawned()
        {
            faceRenderer.sprite = null;
            backRenderer.enabled = false;
            faceRenderer.enabled = false;
        }
    }
}
