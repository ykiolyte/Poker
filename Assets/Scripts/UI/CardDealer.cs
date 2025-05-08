using System.Collections;
using UnityEngine;

public class CardDealer : MonoBehaviour
{
    [SerializeField] private Transform deckPosition;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform[] playerCardSlots;

    public float dealDuration = 0.3f;

    public void DealCardToPlayer(int playerIndex, Sprite cardSprite)
    {
        var cardGO = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity, transform);
        var view = cardGO.GetComponent<CardView>();
        view.SetSprite(cardSprite);

        StartCoroutine(AnimateMove(cardGO.transform, playerCardSlots[playerIndex].position));
    }

    private IEnumerator AnimateMove(Transform card, Vector3 target)
    {
        Vector3 start = card.position;
        float time = 0f;

        while (time < dealDuration)
        {
            time += Time.deltaTime;
            card.position = Vector3.Lerp(start, target, time / dealDuration);
            yield return null;
        }

        card.position = target;
    }
}
