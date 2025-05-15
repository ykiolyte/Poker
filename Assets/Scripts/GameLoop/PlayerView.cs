// Assets/Scripts/GameLoop/PlayerView.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Poker.Gameplay.Factories;
using Poker.UI;

namespace Poker.GameLoop
{
    /// <summary> Отрисовывает две карманные карты игрока. </summary>
    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField] private List<Transform> cardSlots = new();  // ровно 2 ссылки

        private readonly List<CardView> spawned = new();

        public async Task ShowCardAsync(CardDataSO data, int slot, CardFactory factory)
        {
            var view = await factory.CreateCardAsync();
            view.transform.SetParent(cardSlots[slot], false);
            view.SetSprite(data.cardFront);
            spawned.Add(view);
        }

        public void ResetView()
        {
            foreach (var v in spawned) v.gameObject.SetActive(false); // вернули в пул
            spawned.Clear();
        }
    }
}
