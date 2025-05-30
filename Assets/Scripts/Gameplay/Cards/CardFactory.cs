using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Poker.Gameplay.Factories
{
    /// <summary>
    /// Генерирует визуальные карты, поддерживает пул и Addressables-префаб.
    /// Теперь умеет сразу привязывать к родителю.
    /// </summary>
    public sealed class CardFactory
    {
        private readonly object pool;
        private readonly GameObject prefab;

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
        }

        /* ---------- ПУБЛИЧНЫЙ API ---------- */

        public CardView3D Create(CardDataSO data) => CreateInternal(data);

        public async Task<CardView3D> CreateAsync(CardDataSO data)
        {
            var view = CreateInternal(data);
            await Task.Yield();
            return view;
        }

        public CardView3D Create(CardDataSO data, Transform parent)
        {
            var view = CreateInternal(data);
            if (view != null && parent != null)
                view.transform.SetParent(parent, worldPositionStays: false);
            return view;
        }

        public async Task<CardView3D> CreateAsync(CardDataSO data, Transform parent)
        {
            var view = CreateInternal(data);
            if (view != null && parent != null)
                view.transform.SetParent(parent, worldPositionStays: false);

            await Task.Yield();
            return view;
        }

        /* ---------- ВНУТРЕННЯЯ ЛОГИКА ---------- */

        private CardView3D CreateInternal(CardDataSO data)
        {
            var view = SpawnView();
            InitView(view, data);
            return view;
        }

        private CardView3D SpawnView()
        {
            // 1) попытка взять из пула
            if (pool != null)
            {
                var tryGet = pool.GetType().GetMethod(
                    "TryGet",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(CardView3D).MakeByRefType() },
                    null);

                if (tryGet != null)
                {
                    var args = new object[] { null };
                    bool ok = (bool)tryGet.Invoke(pool, args);
                    if (ok && args[0] is CardView3D cv) return cv;
                }
            }

            // 2) Instantiate префаба
            if (prefab != null)
            {
                var go = Object.Instantiate(prefab);
                var cv = go.GetComponent<CardView3D>();
                if (cv != null) return cv;

                Debug.LogWarning("CardFactory: Префаб не содержит CardView3D — fallback.");
            }

            // 3) Заглушка
            var fallback = new GameObject("CardView3D (fallback)");
            return fallback.AddComponent<CardView3D>();
        }

        private static void InitView(Component view, CardDataSO data)
        {
            if (view == null || data == null) return;

            var type = view.GetType();

            // 1) Метод Initialize(CardDataSO)
            var mInit = type.GetMethod("Initialize",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, new[] { typeof(CardDataSO) }, null);

            if (mInit != null)
            {
                mInit.Invoke(view, new object[] { data });
                return;
            }

            // 2) Метод SetData(CardDataSO)
            var mSet = type.GetMethod("SetData",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, new[] { typeof(CardDataSO) }, null);

            mSet?.Invoke(view, new object[] { data });
        }
    }
}
