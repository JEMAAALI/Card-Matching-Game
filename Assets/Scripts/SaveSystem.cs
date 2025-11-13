using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CardData
{
    public int cardValue;
    public string faceSpriteName;
    public string backSpriteName;
    public bool isFlipped;
}

[System.Serializable]
public class GameData
{
    public int rows;
    public int cols;
    public float timeRemaining;
    public int turns;
    public int matches;
    public int score; // <--- add this
    public List<CardData> cards = new List<CardData>();
}

public class SaveSystem : MonoBehaviour
{

    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

  
    public void SaveGame(GameManager gameManager, List<CardsController> allCards)
    {
        GameData data = new GameData
        {
            rows = GameSettings.Rows,
            cols = GameSettings.Cols,
            timeRemaining = gameManager.TimeRemaining,
            turns = gameManager.Turns,
            matches = gameManager.Matches,
            cards = new List<CardData>()
        };

        foreach (var card in allCards)
        {
            data.cards.Add(new CardData
            {
                cardValue = card.cardValue,
                faceSpriteName = card.cardFace.name,
                backSpriteName = card.cardBack.name,
                isFlipped = card.isFlipped
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"💾 Game saved to {savePath}");
    }

    public GameData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("⚠ No save file found.");
            return null;
        }

        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);
        Debug.Log($"📂 Game loaded from {savePath}");
        return data;
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    /// <summary>
    /// Helper: Get Sprite by name from Resources folder
    /// </summary>
    public Sprite GetSpriteByName(string spriteName, List<Sprite> availableSprites)
    {
        foreach (var s in availableSprites)
        {
            if (s.name == spriteName)
                return s;
        }
        Debug.LogWarning($"⚠ Sprite {spriteName} not found in available sprites.");
        return null;
    }
}
