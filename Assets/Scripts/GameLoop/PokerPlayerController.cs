// Assets/Scripts/GameLoop/PokerPlayerController.cs
using System.Threading.Tasks;
using UnityEngine;
using Poker.Gameplay.Cards;
using Poker.Gameplay.Factories;
using Poker.UI;

namespace Poker.GameLoop
{
    [RequireComponent(typeof(PlayerView))]
    public sealed class PokerPlayerController : MonoBehaviour
    {
        [SerializeField] private int startStack = 1000;

        public PokerPlayerModel Model { get; private set; }
        private PlayerView view;

        private void Awake()
        {
            view  = GetComponent<PlayerView>();
            Model = new PokerPlayerModel(GetInstanceID(), startStack);
        }

        public async Task DealHoleCardAsync(CardDataSO card, CardFactory factory)
        {
            Model.DealCard(card);
            await view.ShowCardAsync(card, Model.HoleCards.Count - 1, factory);
        }

        public void ResetForNewHand()
        {
            Model.ResetRound();
            view.ResetView();
        }
    }
}
