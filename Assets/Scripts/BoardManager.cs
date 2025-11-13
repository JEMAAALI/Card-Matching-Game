using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private List<Sprite> cardFaceSprites;
    [SerializeField] private GridLayoutGroup layoutGroup;
    [SerializeField] private GameObject winEffect; // assign in inspector
    public int TotalPairs { get; private set; }

    private List<CardsController> allCards = new List<CardsController>();
    public List<Sprite> CardFaceSprites => cardFaceSprites;
    public Sprite CardBackSprite => cardBackSprite;

    public void SetupBoard(int rows, int cols, List<CardData> savedCards)
    {
        ClearBoard();

        int totalCards = rows * cols;
        TotalPairs = totalCards / 2;

        List<int> cardValues = new List<int>();

        for (int i = 0; i < totalCards; i++)
        {
            int value = savedCards != null ? savedCards[i].cardValue : i / 2;
            cardValues.Add(value);
        }

        if (savedCards == null)
            Shuffle(cardValues);

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            var card = cardObj.GetComponent<CardsController>();

            // Set sprites
            Sprite faceSprite = savedCards != null
                ? GetSpriteByName(savedCards[i].faceSpriteName, CardFaceSprites)
                : CardFaceSprites[cardValues[i]];

            Sprite backSprite = savedCards != null
                ? GetSpriteByName(savedCards[i].backSpriteName, new List<Sprite> { CardBackSprite })
                : CardBackSprite;

            card.SetCard(cardValues[i], backSprite, faceSprite);

            // Flipped visual state will be handled in GameManager.LoadGame()
            allCards.Add(card);
        }

        UpdateGrid(rows, cols);
    }

    private Sprite GetSpriteByName(string spriteName, List<Sprite> availableSprites)
    {
        foreach (var s in availableSprites)
        {
            if (s.name == spriteName)
                return s;
        }
        Debug.LogWarning($"⚠ Sprite {spriteName} not found in available sprites.");
        return null;
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void UpdateGrid(int rows, int cols)
    {
        float layoutWidth = layoutGroup.GetComponent<RectTransform>().rect.width;
        float layoutHeight = layoutGroup.GetComponent<RectTransform>().rect.height;

        float maxCellWidth = layoutWidth / cols;
        float maxCellHeight = layoutHeight / rows;
        float cellSize = Mathf.Min(maxCellWidth, maxCellHeight);

        layoutGroup.cellSize = new Vector2(cellSize, cellSize);
        layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layoutGroup.constraintCount = cols;
    }

    public void ClearBoard()
    {
        foreach (Transform child in cardParent)
            Destroy(child.gameObject);
        allCards.Clear();
    }

    public List<CardsController> GetAllCards() => allCards;

    public void ShowWinEffect()
    {
        if (winEffect != null)
            winEffect.SetActive(true);
    }
}
