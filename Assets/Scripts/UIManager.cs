using UnityEngine;
using UnityEngine.UI;
using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text matchText;
    [SerializeField] private Text turnText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text messageText;
    [SerializeField] private GameObject blockUI;

    BoardManager boardManager;
    public void UpdateUI(int matches, int turns, int score, float time)
    {
        matchText.text = $"Matches: {matches}";
        turnText.text = $"Turns: {turns}";
        scoreText.text = $"Score: {score}";
        UpdateTimer(time);
    }

    public void UpdateTimer(float time)
    {
        time = Mathf.Max(0, time);
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void ShowMessage(string msg)
    {
        messageText.text = msg;
    }

    public void HideCardUI()
    {
        matchText.text ="";
        turnText.text = "";
        scoreText.text = "";
    }

    public void ShowBlockUI()
    {
        blockUI.gameObject.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void Replay()
    {
        Time.timeScale = 1;
        GameSettings.LoadGame = false;
        DontDestroyOnLoad(MenuManager.Instance);
        SceneManager.LoadScene("mainScene");
    }
    public void Quit()
    {
        Destroy(MenuManager.Instance.gameObject);
        Time.timeScale = 1;
        SceneManager.LoadScene("mainMenu");
    }
}
