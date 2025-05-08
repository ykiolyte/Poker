using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Image cardImage;

    public void SetSprite(Sprite sprite)
    {
        cardImage.sprite = sprite;
    }
}
