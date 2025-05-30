using UnityEngine;

/// <summary>
/// Управляет материалами карты, назначает лицевую сторону по Sprite.
/// </summary>
public sealed class CardView3D : MonoBehaviour
{
    [SerializeField] private MeshRenderer front;
    [SerializeField] private MeshRenderer back;

    public void Initialize(CardDataSO data)
    {
        if (front != null && data.cardFront != null)
        {
            front.material = new Material(front.material); // instance, не shared
            front.material.mainTexture = data.cardFront.texture;
        }

        if (back != null && data.cardBack != null)
        {
            back.material = new Material(back.material);
            back.material.mainTexture = data.cardBack.texture;
        }

        ShowBack(); // по умолчанию рубашка
    }

    public void ShowFront() => transform.localRotation = Quaternion.identity;
    public void ShowBack () => transform.localRotation = Quaternion.Euler(0, 180, 0);
}
