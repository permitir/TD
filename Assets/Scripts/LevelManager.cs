using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Path Setup")]
    public Transform startPoint; //Where enemies start
    public Transform[] path; //Waypoints the enemies will follow

    [Header("Game")]
    public int currency; //Currency to buy & upgrade turrets
    public int playerLives = 20;
    public int currentLives;
    public int maxwave = 2;

    [Header("Game State")]
    public bool isGameOver = false; //tracks if player lost.
    public bool isGameWon = false; //tracks if player won.

    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameWonUI;
    [SerializeField] private TextMeshProUGUI waveUI;


    private void Awake()
    {
        main = this;
    }

    public void Start()
    {
        currency = 150;
        currentLives = playerLives;
    }

    public void WaveTextUI(int currentWave)
    {
        waveUI.text = currentWave + "/" + maxwave;
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void CheckWin(int currentWave)
    {
        if (currentWave > maxwave && !isGameOver && !isGameWon)
        {
            Win();
        }
    }

    public void Win()
    {
        isGameWon = true;
        isGameOver = true;
        Debug.Log("Game Won!");

        Time.timeScale = 0f;

        if (gameWonUI != null)
        {
            gameWonUI.SetActive(true);
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Lost!");

        Time.timeScale = 0f;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }


    public void IncreaseCurrency(int amount)
    {
        //Increase user currency after enemy killed
        currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        //How to buy the turrets/upgrades
        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }
        else
        {
            Debug.Log("You have no money to buy this item.");
            return false;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // reset timer
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        //Closes game (ALt + f4)
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; //reset incase game paused
        SceneManager.LoadScene("MainMenu");
    }

    internal string WaveTextUI()
    {
        throw new NotImplementedException();
    }
}
