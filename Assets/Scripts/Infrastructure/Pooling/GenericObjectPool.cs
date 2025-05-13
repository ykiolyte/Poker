using System.Collections.Generic;
using UnityEngine;

namespace Poker.Infrastructure.Pooling
{
    /// <summary>
    /// Универсальный пул объектов. Создаёт начальный запас, возвращает
    /// через <c>Spawn()</c> и прячет через <c>Despawn()</c>.
    /// </summary>
    /// <typeparam name="T">MonoBehaviour, реализующий IPoolable</typeparam>
    public sealed class GenericObjectPool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
    {
        [SerializeField] private T prefab;
        [SerializeField] private int initialSize = 32;

        private readonly Queue<T> objects = new Queue<T>();

        private void Awake()
        {
            for (var i = 0; i < initialSize; i++)
                CreateInstance();
        }

        public T Spawn()
        {
            if (objects.Count == 0) CreateInstance();

            var obj = objects.Dequeue();
            obj.OnSpawned();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Despawn(T obj)
        {
            obj.OnDespawned();
            obj.gameObject.SetActive(false);
            objects.Enqueue(obj);
        }

        private void CreateInstance()
        {
            var instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            objects.Enqueue(instance);
        }
    }

    /// <summary>
    /// Интерфейс для уведомлений об изменении состояния объекта в пуле.
    /// </summary>
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }
}
