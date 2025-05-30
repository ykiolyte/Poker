using System.Collections;
using Poker.Gameplay.Factories;
using UnityEngine;

public class TableView : MonoBehaviour
{
    [SerializeField] private Transform boardParent;
    [SerializeField] private Transform[] boardSlots; // ← добавь массив на 5 слотов
    [SerializeField] private CardFactory fallbackFactory;

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

        int slotIndex = boardParent.childCount;
        if (slotIndex < boardSlots.Length)
        {
            view.transform.SetParent(boardSlots[slotIndex], false);
            view.transform.localPosition = Vector3.zero;
            view.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            Debug.LogWarning($"TableView: превышено число boardSlots: {slotIndex}");
            view.transform.SetParent(boardParent, false);
        }

        // Анимация переворота
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

    public void ResetBoard()
    {
        if (boardParent == null) return;
        foreach (Transform child in boardParent)
            Destroy(child.gameObject);
    }
}
