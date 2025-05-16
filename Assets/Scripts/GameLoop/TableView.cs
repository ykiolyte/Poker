using System.Collections;
using UnityEngine;
using Poker.Gameplay.Cards;
using Poker.Gameplay.Factories;

namespace Poker.GameLoop
{
    public class TableView : MonoBehaviour
    {
        [SerializeField] private Transform   boardParent;   // 5 слотов борда
        [SerializeField] private CardFactory fallbackFactory; // опционально через инспектор

        /* ---------- вызывается из RoundManager ---------- */
        public IEnumerator ShowBoardCardAsync(CardDataSO data, CardFactory runtimeFactory)
        {
            var factory = runtimeFactory ?? fallbackFactory;
            if (factory == null)
            {
                Debug.LogError("TableView: CardFactory не задан — карта борда не будет создана");
                yield break;
            }

            var task = factory.CreateAsync(data);
            while (!task.IsCompleted) yield return null;
            var view = task.Result;

            if (boardParent == null)
            {
                Debug.LogError("TableView: boardParent не назначен");
                yield break;
            }

            view.transform.SetParent(boardParent, false);

            // разворот рубашка → лицевая сторона
            const float duration = 0.3f;
            view.transform.localRotation = Quaternion.Euler(0, 180, 0);
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float y = Mathf.Lerp(180f, 0f, t / duration);
                view.transform.localRotation = Quaternion.Euler(0, y, 0);
                yield return null;
            }
            view.transform.localRotation = Quaternion.identity;
        }

        /* ---------- очистка перед новой раздачей ---------- */
        public void ResetBoard()
        {
            if (boardParent == null) return;
            foreach (Transform child in boardParent)
                Destroy(child.gameObject);
        }
    }
}
