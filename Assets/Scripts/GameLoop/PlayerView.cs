using System.Threading.Tasks;
using UnityEngine;
using Poker.Gameplay.Cards;
using Poker.Gameplay.Factories;

namespace Poker.GameLoop
{
    [RequireComponent(typeof(Transform))]
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Transform   holeCardParent;
        [SerializeField] private CardFactory cardFactory;      // резервный, можно задавать в инспекторе

        /* ---------- вызовы из PokerPlayerController ---------- */

        /// <summary>
        /// Асинхронно создаёт визуал карты, прикрепляет к руке игрока
        /// и ставит её на позицию <paramref name="handIndex"/> (0 — левая, 1 — правая).
        /// </summary>
        public async Task ShowCardAsync(CardDataSO card, int handIndex, CardFactory externalFactory)
        {
            var factory = externalFactory ?? cardFactory;
            if (factory == null)
            {
                Debug.LogError("PlayerView: CardFactory == null, не могу создать карту.");
                return;
            }

            var view = await factory.CreateAsync(card);
            view.transform.SetParent(holeCardParent, false);

            // простая раскладка: горизонтальный сдвиг ±0.3 м
            float offset = 0.3f * (handIndex == 0 ? -1 : 1);
            view.transform.localPosition = new Vector3(offset, 0f, 0f);
            view.transform.localRotation = Quaternion.Euler(0, 180, 0); // рубашкой вверх

            // анимация переворота
            const float dur = 0.25f;
            for (float t = 0; t < dur; t += Time.deltaTime)
            {
                float y = Mathf.Lerp(180, 0, t / dur);
                view.transform.localRotation = Quaternion.Euler(0, y, 0);
                await Task.Yield();
            }
            view.transform.localRotation = Quaternion.identity;
        }

        /// <summary>Очищает руку перед новой раздачей.</summary>
        public void ResetView()
        {
            foreach (Transform child in holeCardParent)
                Destroy(child.gameObject);
        }
    }
}
