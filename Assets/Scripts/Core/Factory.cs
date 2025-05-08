using UnityEngine;

public static class Factory
{
    public static GameObject CreateCard(CardDataSO data, Transform parent)
    {
        var card = new GameObject($"{data.rank} of {data.suit}");
        var spriteRenderer = card.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = data.cardFront;
        card.transform.SetParent(parent);
        return card;
    }

    public static GameObject CreateAvatar(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Object.Instantiate(prefab, position, rotation);
    }
}
