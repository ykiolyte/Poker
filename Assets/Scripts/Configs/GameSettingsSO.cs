using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/Game Settings")]
public class GameSettingsSO : ScriptableObject
{
    [Header("Blinds")]
    public int smallBlind = 50;
    public int bigBlind = 100;

    [Header("Player")]
    public int startingStack = 10000;
    public int maxPlayers = 6;
}
