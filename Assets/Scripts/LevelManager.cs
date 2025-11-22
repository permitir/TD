using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Path Setup")]
    public Transform startPoint; //Where enemies start
    public Transform[] path; //Waypoints the enemies will follow

    [Header("Game")]
    public int currency = 150; //Currency to buy & upgrade turrets
    public int playerLives = 20;
    public int currentLives;
    public int maxwave = 50;
    public int currentLevelNumber = 1; //individually set for every level 👍

    [Header("Game State")]
    public bool isGameOver = false; //tracks if player lost.
    public bool isGameWon = false; //tracks if player won.

    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameWonUI;
    [SerializeField] private TextMeshProUGUI waveUI;
    [SerializeField] private TextMeshProUGUI livesText;


    private void Awake()
    {
        main = this;
    }

    public void Start()
    {
        currentLives = playerLives;
        LivesTextUI(); // Instantly update the users life.
    }

    public void WaveTextUI(int currentWave)
    {
        if (waveUI != null)
        {
            waveUI.text = currentWave + "/" + maxwave;

            // Colour scheme so it is more user friendly
            if (currentWave <= maxwave / 5)
                waveUI.color = Color.green;
            else if (currentWave <= maxwave / 2)
                waveUI.color = Color.yellow;
            else
                waveUI.color = Color.red;
        }
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;
        LivesTextUI();

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void LivesTextUI()
    {
        if (livesText != null)
        {
            livesText.text = currentLives + "/" + playerLives;

            // Colour scheme so it is more user friendly
            if (currentLives <= playerLives / 5)
                livesText.color = Color.red;
            else if (currentLives <= playerLives / 2)
                livesText.color = Color.yellow;
            else
                livesText.color = Color.green;
        }
    }

    public void CheckWin(int currentWave)
    {
        if (currentWave >= maxwave && !isGameOver && !isGameWon)
        {
            Debug.Log("Game Won!");
            Win();
        }
    }

    public void Win()
    {
        isGameWon = true;
        Debug.Log("Game Condition has been met to win.");

        // Save level completion
        SaveSystem.instance.CompleteLevel(currentLevelNumber);

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
        SaveSystem.instance.AddCurrency(amount); // Saves how much money the user has ever made playing the game
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

    public void NextLevel()
    {
        Time.timeScale = 1f; // reset time

        // load next level by getting next scene index ohh yeahhh vector
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            LoadMainMenu();
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