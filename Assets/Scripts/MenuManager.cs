using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("UI")]
    public Text A1Text;
    public Text A2Text;
    public Button A1IncrementButton;
    public Button A1DecrementButton;
    public Button A2IncrementButton;
    public Button A2DecrementButton;
    public Button loadButton;

    [Header("Defaults")]
    public int[] A1 = { 2, 3, 4, 5, 6, 7, 8, 9 };
    public int[] A2 = { 2, 3, 4, 5, 6, 7, 8, 9 };

    private List<int> validA1Selections = new List<int>();
    private List<int> validA2Selections = new List<int>();

    private int currentA1Index = 0;
    private int currentA2Index = 0;

    public int rows;
    public int cols;

    public SaveSystem saveSystem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // ensure single menu manager
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        // initialize lists and UI
        validA1Selections.AddRange(A1);
        validA2Selections.AddRange(A2);

        A1IncrementButton.onClick.AddListener(IncrementA1);
        A1DecrementButton.onClick.AddListener(DecrementA1);
        A2IncrementButton.onClick.AddListener(IncrementA2);
        A2DecrementButton.onClick.AddListener(DecrementA2);

        // load saved availability of 'Load' button
        string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        loadButton.interactable = File.Exists(savePath);

        UpdateA1Text();
        UpdateA2Text();
    }

    private void IncrementA1()
    {
        do
        {
            currentA1Index = (currentA1Index + 1) % validA1Selections.Count;
        } while (!IsValidProduct(validA1Selections[currentA1Index], int.Parse(A2Text.text)));

        UpdateA1Text();
        UpdateValidA2Selections();
    }

    private void DecrementA1()
    {
        do
        {
            currentA1Index = (currentA1Index - 1 + validA1Selections.Count) % validA1Selections.Count;
        } while (!IsValidProduct(validA1Selections[currentA1Index], int.Parse(A2Text.text)));

        UpdateA1Text();
        UpdateValidA2Selections();
    }

    private void IncrementA2()
    {
        do
        {
            currentA2Index = (currentA2Index + 1) % validA2Selections.Count;
        } while (!IsValidProduct(int.Parse(A1Text.text), validA2Selections[currentA2Index]));

        UpdateA2Text();
        UpdateValidA1Selections();
    }

    private void DecrementA2()
    {
        do
        {
            currentA2Index = (currentA2Index - 1 + validA2Selections.Count) % validA2Selections.Count;
        } while (!IsValidProduct(int.Parse(A1Text.text), validA2Selections[currentA2Index]));

        UpdateA2Text();
        UpdateValidA1Selections();
    }

    private void UpdateA1Text()
    {
        A1Text.text = validA1Selections[currentA1Index].ToString();
    }

    private void UpdateA2Text()
    {
        A2Text.text = validA2Selections[currentA2Index].ToString();
    }

    private void UpdateValidA1Selections()
    {
        validA1Selections.Clear();
        int selectedA2 = int.Parse(A2Text.text);
        foreach (int value in A1)
            if (IsValidProduct(value, selectedA2)) validA1Selections.Add(value);

        currentA1Index = validA1Selections.IndexOf(int.Parse(A1Text.text));
        if (currentA1Index < 0) currentA1Index = 0;
        UpdateA1Text();
    }

    private void UpdateValidA2Selections()
    {
        validA2Selections.Clear();
        int selectedA1 = int.Parse(A1Text.text);
        foreach (int value in A2)
            if (IsValidProduct(selectedA1, value)) validA2Selections.Add(value);

        currentA2Index = validA2Selections.IndexOf(int.Parse(A2Text.text));
        if (currentA2Index < 0) currentA2Index = 0;
        UpdateA2Text();
    }

    private bool IsValidProduct(int a1, int a2)
    {
        int product = a1 * a2;
        return product <= 30 && product % 2 == 0;
    }

    public void Play()
    {
        rows = int.Parse(A1Text.text);
        cols= int.Parse(A2Text.text);
        GameSettings.Rows = int.Parse(A1Text.text);
        GameSettings.Cols = int.Parse(A2Text.text);
        GameSettings.LoadGame = false;
        DontDestroyOnLoad(MenuManager.Instance);
        SceneManager.LoadScene("mainScene");
    }

    public void Load()
    {
        GameSettings.LoadGame = true;
        Time.timeScale = 1.0f;
        DontDestroyOnLoad(MenuManager.Instance);
        SceneManager.LoadScene("mainScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
