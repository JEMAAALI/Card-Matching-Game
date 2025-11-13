using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Board Settings")]
    public int rows = 4;
    public int cols = 4;

    [Header("Gameplay Settings")]
    public float timeLimit = 3f;      // Total time for game in seconds
    public int pointsPerMatch = 10;     // Points awarded per matched pair
}
