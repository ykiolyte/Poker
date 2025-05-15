using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "Configs/UI Config")]
public class UIConfigSO : ScriptableObject
{
    public Color actionHighlightColor = Color.green;
    public Color disabledButtonColor = Color.gray;
    public Font mainFont;
}