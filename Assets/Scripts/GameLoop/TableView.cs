// Assets/Scripts/GameLoop/TableView.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Poker.Gameplay.Factories;
using Poker.UI;
using Poker.Gameplay.Cards;

namespace Poker.GameLoop
{
    /// <summary>Управляет визуалом и хранит список community-карт.</summary>
    public sealed class TableView : MonoBehaviour
    {
        [SerializeField, Tooltip("5 слотов: flop0–2, turn, river")]
        private List<Transform> boardSlots = new List<Transform>();

        private readonly List<CardView> _spawned = new List<CardView>();
        private readonly List<CardDataSO> currentCards = new List<CardDataSO>();
        /// <summary>Текущий борд (в порядке выкладки).</summary>
        public IReadOnlyList<CardDataSO> CurrentCards => currentCards;

        /// <summary>Показывает следующую карту на борде и запоминает её в currentCards.</summary>
        public async Task ShowBoardCardAsync(CardDataSO data, CardFactory factory)
        {
            if (data == null) return;

            int slotIndex = currentCards.Count;              // куда кладём эту карту
            var view = await factory.CreateCardAsync();
            view.transform.SetParent(boardSlots[slotIndex], false);
            view.SetSprite(data.cardFront);
            _spawned.Add(view);

            currentCards.Add(data);                          // теперь не забываем запомнить карту
        }

        /// <summary>Убирает все карты с борда и чистит список.</summary>
        public void ResetBoard()
        {
            foreach (var v in _spawned)
                v.gameObject.SetActive(false);
            _spawned.Clear();
            currentCards.Clear();                            // чистим именно здесь
        }
    }
}
