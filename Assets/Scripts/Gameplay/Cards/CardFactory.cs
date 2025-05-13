using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using Poker.Gameplay.Cards;
using Poker.UI;

namespace Poker.Gameplay.Factories
{
    /// <summary>
    /// Создаёт карты через Addressables + использует пул для переиспользования.
    /// Работает асинхронно: сначала убеждается, что Asset загружен,
    /// потом запрашивает объект из пула.
    /// </summary>
    public sealed class CardFactory
    {
        private readonly CardPool cardPool;
        private readonly AsyncOperationHandle<GameObject> handle;

        public CardFactory(CardPool pool, AssetReference cardAsset)
        {
            cardPool = pool;
            handle = Addressables.LoadAssetAsync<GameObject>(cardAsset);
        }

        public async Task<CardView> CreateCardAsync()
        {
            if (!handle.IsDone) await handle.Task;     // дожидаемся загрузки префаба
            return cardPool.GetCard();
        }

        public void ReleaseCard(CardView view) => cardPool.ReturnCard(view);
    }
}
