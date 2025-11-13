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
    public SaveSystem saveSystem;
    public UIManager uiManager;
    public AudioManager audioManager;

    [Header("Settings")]
    public GameConfig config;
    public int pointsPerMatch = 10; // added here since GameConfig doesn't have it

    // Game state
    private List<CardsController> flippedCards = new List<CardsController>();
    public int Turns { get; private set; } = 0;
    public int Score { get; private set; } = 0;
    public int Matches { get; private set; } = 0;
    public float TimeRemaining { get; set; } = 180;

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

        if (GameSettings.LoadGame && saveSystem.HasSave())
        {
            LoadGame();
        }
        else
        {
            int rows = MenuManager.Instance.rows;
            int cols = MenuManager.Instance.cols;
            StartNewGame(rows, cols);
        }
         
    }

    public void StartNewGame(int rows, int cols)
    {
        //int rowsx = GameSettings.Rows > 0 ? GameSettings.Rows : config.rows;
        //int colsx = GameSettings.Cols > 0 ? GameSettings.Cols : config.cols;
        int rowsx = MenuManager.Instance.rows;
        int colsx = MenuManager.Instance.cols;
        boardManager.SetupBoard(rowsx, colsx,null);
        allCards = boardManager.GetAllCards();
        Debug.Log($"New game started with {rows}x{cols} cards.");
    }


    private void LoadGame()
    {
        var data = saveSystem.LoadGame();
        if (data == null)
        {
            Debug.Log("Save not found, starting new game.");
            StartNewGame(config.rows, config.cols);
            return;
        }

        MenuManager.Instance.rows = data.rows;
        MenuManager.Instance.cols = data.cols;

        // Restore stats
        Turns = data.turns;
        Score = data.matches * config.pointsPerMatch;
        Matches = data.matches;
        TimeRemaining = data.timeRemaining;

        // Setup board with saved cards
        boardManager.SetupBoard(data.rows, data.cols, data.cards);
        allCards = boardManager.GetAllCards();

        flippedCardss.Clear();
        matchedCards.Clear();

        List<CardsController> unmatchedFlippedCards = new List<CardsController>();

        // Go through all saved cards
        for (int i = 0; i < data.cards.Count; i++)
        {
            var cardData = data.cards[i];
            var card = allCards[i];

            if (IsCardMatched(cardData, data))
            {
                // Matched cards → keep flipped, disable interaction
                matchedCards.Add(card);
                card.LoadFlipped();  // visually flipped
                card.DisableCard();  // cannot click
                card.ShowSparkle();  // optional
            }
            else if (cardData.isFlipped)
            {
                // Add unmatched flipped cards to a temporary list
                unmatchedFlippedCards.Add(card);
            }
            else
            {
                // Normal unflipped card
                card.HideCardImmediate();
                card.EnableCard();
            }
        }

        // Show unmatched flipped cards together for 0.5s, then flip back
        StartCoroutine(FlipBackUnmatchedCards(unmatchedFlippedCards));

        // Update UI
        uiManager?.UpdateUI(Matches, Turns, Score, TimeRemaining);

        Debug.Log($"Game loaded. Matched cards: {matchedCards.Count}");
    }
    private IEnumerator FlipBackUnmatchedCards(List<CardsController> cards)
    {
        if (cards.Count == 0) yield break;

        // Show all unmatched flipped cards visually
        foreach (var card in cards)
        {
            card.LoadFlipped();  // show face
            card.DisableCard();   // temporarily prevent click
        }

        // Wait for 0.5 seconds so player sees them together
        yield return new WaitForSeconds(1.5f);

        // Flip them back and make clickable
        foreach (var card in cards)
        {
            card.HideCardImmediate(); // flip back
            card.EnableCard();        // enable clicking
        }
    }

    private bool IsCardMatched(CardData cardData, GameData data)
    {
        // Count all cards with same value that were flipped in save
        int countFlippedSameValue = 0;
        foreach (var c in data.cards)
        {
            if (c.cardValue == cardData.cardValue && c.isFlipped)
                countFlippedSameValue++;
        }

        // Matched pair = both flipped
        return countFlippedSameValue >= 2;
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

            audioManager.PlayMatch();
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

            audioManager.PlayNotMatch();
            card1.HideCard();
            card2.HideCard();

            card1.EnableCard();
            card2.EnableCard();
        }

        if (uiManager != null)
            uiManager.UpdateUI(Matches, Turns, Score, TimeRemaining);

        CheckForWin();
    }

    private void CheckForWin()
    {
        if (matchedCards.Count == allCards.Count)
        {
            EventBus.RaiseWin();
            stopTimer = true;
            uiManager?.HideCardUI();
            uiManager.ShowMessage("Congratulations! You win");
            boardManager?.ShowWinEffect();
            audioManager.PlayWin();
            Debug.Log("All cards matched! You win!");
        }
    }

    public void SaveGame()
    {
        if (saveSystem != null)
            saveSystem.SaveGame(this, allCards);
    }

    public void QuitGame()
    {
        SaveGame();
        Application.Quit();
    }

    private void Update()
    {
        if (TimeRemaining > 0)
        {
            if (!stopTimer)
            {
                TimeRemaining -= Time.deltaTime;
                uiManager?.UpdateUI(Matches, Turns, Score, TimeRemaining);
            }
        }
        else
        {
            TimeRemaining = 0; 
            EventBus.RaiseGameOver();
            uiManager.HideCardUI();
            if (!gameOverSoundPlayed)
            {
                uiManager.ShowMessage("Game Over!       Time UP!");
                uiManager.ShowBlockUI(); 
                audioManager?.PlayLose(); // Play once
                gameOverSoundPlayed = true;
            }
        }
    }
}
