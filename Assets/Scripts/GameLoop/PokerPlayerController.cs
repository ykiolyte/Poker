// Assets/Scripts/GameLoop/PokerPlayerController.cs
using System.Threading.Tasks;
using UnityEngine;
using Poker.Gameplay.Factories;
using Poker.Gameplay.Cards;

namespace Poker.GameLoop
{
    [RequireComponent(typeof(PlayerView))]

    public sealed class PokerPlayerController : MonoBehaviour
    {
        [SerializeField] private int startStack = 1000;

        [Header("Card Fan Settings")]
        [SerializeField] private float cardSpacing = 0.03f;
        [SerializeField] private float cardAngle = 5f;

        public PokerPlayerModel Model { get; private set; }
        private PlayerView view;

        private void Awake()
        {
            view = GetComponent<PlayerView>();
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

        public Transform GetCardAnchor(bool isLeftHand = false)
        {
            return GetComponent<CardAnchorProvider>()?.GetAnchor(isLeftHand);
        }

        /// <summary>Настраиваемая раскладка карт веером.</summary>
        public void ApplyFanOffset(Transform cardTransform, int index)
        {
            float offsetX = (index - 0.5f) * cardSpacing;
            float rotationY = (index - 0.5f) * cardAngle;

            cardTransform.localPosition += new Vector3(offsetX, 0f, 0f);
            cardTransform.localRotation = Quaternion.Euler(0f, rotationY, 0f);
        }

    }
    
}
