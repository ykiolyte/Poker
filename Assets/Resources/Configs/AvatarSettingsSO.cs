using UnityEngine;

[CreateAssetMenu(fileName = "AvatarSettings", menuName = "Configs/Avatar Settings")]
public class AvatarSettingsSO : ScriptableObject
{
    public GameObject[] avatarPrefabs;
    public float avatarScale = 1.0f;
}