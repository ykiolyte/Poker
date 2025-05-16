using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Poker.Gameplay.Cards;
using Poker.UI;

namespace Poker.Gameplay.Factories
{
    /// <summary>
    /// Генерирует визуальные карточки.
    /// • Пул объектов (любого типа) — ищем методом TryGet(out CardView) через рефлексию.
    /// • Если задан префаб Addressables — Instantiate.
    /// • Иначе создаём заглушку.
    /// Для установки данных карты ищем публичный/приватный метод Initialize(CardDataSO)
    /// или SetData(CardDataSO) через рефлексию.
    /// </summary>
    public sealed class CardFactory
    {
        private readonly object     pool;    // можно передать любой пул
        private readonly GameObject prefab;  // может быть null

        public CardFactory(object pool, AssetReference cardPrefabRef)
        {
            this.pool = pool;

            if (cardPrefabRef != null && cardPrefabRef.RuntimeKeyIsValid())
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(cardPrefabRef);
                prefab = handle.WaitForCompletion();
                if (prefab == null)
                    Debug.LogError("CardFactory: не удалось загрузить префаб карты через Addressables.");
            }
            else
            {
                Debug.LogWarning("CardFactory: AssetReference не задан или некорректен; используем только пул/заглушки.");
            }
        }

        /// <summary>Синхронное создание для редактора.</summary>
        public CardView Create(CardDataSO data) => CreateInternal(data);

        /// <summary>Асинхронное создание для корутин.</summary>
        public async Task<CardView> CreateAsync(CardDataSO data)
        {
            var view = CreateInternal(data);
            await Task.Yield();
            return view;
        }

        private CardView CreateInternal(CardDataSO data)
        {
            var view = SpawnView();
            InitView(view, data);
            return view;
        }

        private CardView SpawnView()
        {
            // 1) Пытаемся взять из пула: ищем метод TryGet(out CardView)
            if (pool != null)
            {
                var mi = pool.GetType().GetMethod(
                    "TryGet",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(CardView).MakeByRefType() },
                    null);

                if (mi != null)
                {
                    var args = new object[] { null };
                    bool ok = (bool)mi.Invoke(pool, args);
                    if (ok && args[0] is CardView cv)
                        return cv;
                }
            }

            // 2) Если есть загруженный префаб — Instantiate
            if (prefab != null)
                return Object.Instantiate(prefab).GetComponent<CardView>();

            // 3) Fallback: новая заглушка, чтобы проект не падал
            var go = new GameObject("CardView (fallback)");
            return go.AddComponent<CardView>();
        }

        private static void InitView(CardView view, CardDataSO data)
        {
            if (view == null || data == null) return;

            var type = view.GetType();

            // ищем Initialize(CardDataSO)
            var mInit = type.GetMethod(
                "Initialize",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { typeof(CardDataSO) },
                null);

            if (mInit != null)
            {
                mInit.Invoke(view, new object[] { data });
                return;
            }

            // ищем SetData(CardDataSO)
            var mSet = type.GetMethod(
                "SetData",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { typeof(CardDataSO) },
                null);

            if (mSet != null)
                mSet.Invoke(view, new object[] { data });
        }
    }
}
