using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the overall game flow: board setup, match checking, saving/loading, win detection.
/// Supports multiple pairs being flipped simultaneously.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    public BoardManager boardManager;


    [Header("Settings")]
    public GameConfig config;
    public int pointsPerMatch = 10; // added here since GameConfig doesn't have it

    // Game state
    private List<CardsController> flippedCards = new List<CardsController>();
    public int Turns { get; private set; } = 0;
    public int Score { get; private set; } = 0;
    public int Matches { get; private set; } = 0;
    public float TimeRemaining { get; set; } = 180f;

    private List<CardsController> allCards = new List<CardsController>();
    private bool gameOverSoundPlayed = false;
    private bool stopTimer = false;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventBus.OnCardFlipped += HandleCardFlipped;
    }

    private void OnDisable()
    {
        EventBus.OnCardFlipped -= HandleCardFlipped;
    }

    private void Start()
    {
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        yield return null; // wait one frame to let scene load

        StartNewGame(config.rows, config.cols);
    }

    public void StartNewGame(int rows, int cols)
    {
        int rowsx = GameSettings.Rows > 0 ? GameSettings.Rows : config.rows;
        int colsx = GameSettings.Cols > 0 ? GameSettings.Cols : config.cols;
        boardManager.SetupBoard(rowsx, colsx);
        allCards = boardManager.GetAllCards();
        Debug.Log($"🟩 New game started with {rows}x{cols} cards.");
    }

     

    /// <summary>
    /// Called by EventBus when a card is flipped.
    /// Handles multiple pairs independently.
    /// </summary>
    /// <param name="card"></param>
    private List<CardsController> flippedCardss = new List<CardsController>();
    private List<CardsController> matchedCards = new List<CardsController>();

    public void HandleCardFlipped(CardsController card)
    {
        flippedCardss.Add(card);

        if (flippedCardss.Count % 2 == 0)
        {
            StartCoroutine(CheckPair(flippedCardss[flippedCardss.Count - 2], flippedCardss[flippedCardss.Count - 1]));
        }
    }

    private IEnumerator CheckPair(CardsController card1, CardsController card2)
    {
        card1.DisableCard();
        card2.DisableCard();

        yield return new WaitForSeconds(0.05f);

        Turns++;


        if (card1.cardValue == card2.cardValue)
        {
            Matches++;
            Score += config.pointsPerMatch;

            card1.ShowSparkle();
            card2.ShowSparkle();

            card1.DisableCard();
            card2.DisableCard();

            matchedCards.Add(card1);
            matchedCards.Add(card2);

            EventBus.RaiseMatch();
        }
        else
        {
            yield return new WaitForSeconds(0.05f);

            card1.HideCard();
            card2.HideCard();

            card1.EnableCard();
            card2.EnableCard();
        }



     } 

   

     
}
