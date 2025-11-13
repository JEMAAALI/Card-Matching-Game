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
     public int TotalPairs { get; private set; }

    private List<CardsController> allCards = new List<CardsController>();

    public void SetupBoard(int rows, int cols)
    {
        ClearBoard();

        int totalCards = rows * cols;
        TotalPairs = totalCards / 2;

        List<int> cardValues = new List<int>();
        for (int i = 0; i < TotalPairs; i++)
        {
            cardValues.Add(i);
            cardValues.Add(i);
        }

        Shuffle(cardValues);

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            var card = cardObj.GetComponent<CardsController>();
            card.SetCard(cardValues[i], cardBackSprite, cardFaceSprites[cardValues[i]]);
            allCards.Add(card);
        }

        UpdateGrid(rows, cols);
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

    
}
