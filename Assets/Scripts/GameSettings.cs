/// <summary>
/// Simple transient settings holder — used to transfer menu selections to the game scene.
/// Cleared/reset after use if desired.
/// </summary>
public static class GameSettings
{
    public static int Rows { get; set; } = 4;
    public static int Cols { get; set; } = 4;
    public static bool LoadGame { get; set; } = false;

    public static void Reset()
    {
        Rows = 4;
        Cols = 4;
        LoadGame = false;
    }
}
