// Assets/Scripts/GameLoop/PlayerView.cs
using System.Threading.Tasks;
using UnityEngine;
using Poker.Gameplay.Cards;
using Poker.Gameplay.Factories;

namespace Poker.GameLoop
{
    /// <summary>
    /// Отвечает только за клиентский визуал двух карманных карт.
    /// cardSlots[0] — первая (левая) карта, cardSlots[1] — вторая (правая).
    /// </summary>
    public sealed class PlayerView : MonoBehaviour
    {
        [Tooltip("CardSlot_0 и CardSlot_1 внутри HandRoot")]
        [SerializeField] private Transform[] cardSlots = new Transform[2];

        [Tooltip("Фабрика, если не передаётся извне")]
        [SerializeField] private CardFactory cardFactory;

        private void Awake()
        {
            if (cardSlots is { Length: 2 } == false ||
                cardSlots[0] == null || cardSlots[1] == null)
            {
                Debug.LogError(
                    "PlayerView: cardSlots не заданы! " +
                    "Укажите оба CardSlot объекта в инспекторе.");
            }
        }

        /// <summary>
        /// Создаёт карту и ставит в нужный слот. <paramref name="handIndex"/> ∈ {0,1}.
        /// </summary>
        public async Task ShowCardAsync(CardDataSO card, int handIndex, CardFactory externalFactory)
        {
            if (handIndex is < 0 or > 1)
            {
                Debug.LogError($"PlayerView: неверный handIndex={handIndex}");
                return;
            }

            var slot = cardSlots[handIndex];
            if (slot == null)
            {
                Debug.LogError("PlayerView: cardSlot == null, карта не будет создана.");
                return;
            }

            var factory = externalFactory ?? cardFactory;
            if (factory == null)
            {
                Debug.LogError("PlayerView: CardFactory == null, карта не будет создана.");
                return;
            }

            // Фабрика сама привяжет карту к slot (worldPositionStays = false)
            var view = await factory.CreateAsync(card, slot);
            if (view == null) return;

            // Изначально рубашкой вверх
            view.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            // Плавный переворот
            const float dur = 0.25f;
            for (float t = 0; t < dur; t += Time.deltaTime)
            {
                float y = Mathf.Lerp(180f, 0f, t / dur);
                view.transform.localRotation = Quaternion.Euler(0f, y, 0f);
                await Task.Yield();
            }
            view.transform.localRotation = Quaternion.identity;
        }

        /// <summary>Удаляет все дочерние GO из обоих слотов.</summary>
        public void ResetView()
        {
            foreach (var s in cardSlots)
                if (s != null)
                    foreach (Transform c in s) Destroy(c.gameObject);
        }
    }
}
