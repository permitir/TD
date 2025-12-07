using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor.Timeline;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Path Setup")]
    public Transform startPoint; // This will locate where the enemies will start
    public Transform[] path; // These are the waypoints the enemies will follow

    [Header("Game")]
    public int currency = 150; //The currency used to buy & upgrade turrets
    public int playerLives = 20;
    public int currentLives;
    public int maxwave = 50;
    public int currentLevelNumber = 1; // This can be individually set for every level 👍

    [Header("Game State")]
    public bool isGameOver = false; // Will track if player lost.
    public bool isGameWon = false; // Will track if player won.

    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameWonUI;
    [SerializeField] private TextMeshProUGUI waveUI;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI NoMoneyText;

    [Header("Game Won UI")]
    [SerializeField] private TextMeshProUGUI winTimeText;
    [SerializeField] private TextMeshProUGUI winEnemiesKilledText;
    [SerializeField] private TextMeshProUGUI winBossesKilledText;

    [Header("Game Over UI")]
    [SerializeField] private TextMeshProUGUI loseTimeText;
    [SerializeField] private TextMeshProUGUI loseEnemiesKilledText;
    [SerializeField] private TextMeshProUGUI loseBossesKilledText;

    // Level stat tracking
    private float levelStartTime;
    private int enemiesKilledThisLevel = 0;
    private int bossesKilledThisLevel = 0;


    private void Awake()
    {
        main = this;
    }

    public void Start()
    {
        if (SaveSystem.instance == null)
        {
            GameObject saveSystemObj = new GameObject("SaveSystem");
            saveSystemObj.AddComponent<SaveSystem>();
        }
        currentLives = playerLives;
        LivesTextUI(); // Instantly update the users life.

        // Track time in level
        levelStartTime = Time.time;

        // Reset stats level
        enemiesKilledThisLevel = 0;
        bossesKilledThisLevel = 0;
    }

    public void EnemyKilled(bool isBoss = false)
    {
        enemiesKilledThisLevel++;

        if (isBoss)
        {
            bossesKilledThisLevel++;
        }
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

        // Calculate time taken to beat level
        float timeTaken = Time.time - levelStartTime;
        UpdateWinStats(timeTaken); // Update stats display

        // Save level completion
        SaveSystem.instance.CompleteLevel(currentLevelNumber);

        Time.timeScale = 0f;

        if (gameWonUI != null)
        {
            gameWonUI.SetActive(true);
        }
    }

    private void UpdateWinStats(float timeTaken)
    {
        // Formats the time as in mins:seconds
        string timeString = FormatTime(timeTaken);

        if (winTimeText != null)
        {
            winTimeText.text = "Time:" + timeString;
        }
        if (winEnemiesKilledText != null)
        {
            winEnemiesKilledText.text = "Enemies killed: " + enemiesKilledThisLevel;
        }
        if (winBossesKilledText != null)
        {
            winBossesKilledText.text = "Bosses killed: " + bossesKilledThisLevel;
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Lost!");

        // Calculate time spent
        float timeSpent = Time.time - levelStartTime;
        UpdateGameOverStats(timeSpent); // Updates game over stats display

        Time.timeScale = 0f;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    private void UpdateGameOverStats(float timeSpent)
    {
        // formats the time as in mins:seconds
        string timeString = FormatTime(timeSpent);

        if (loseTimeText != null)
        {
            loseTimeText.text = "Time survived: " + timeString;
        }
        if (loseEnemiesKilledText != null)
        {
            loseEnemiesKilledText.text =  "Enemies killed: " + enemiesKilledThisLevel;
        }
        if (loseBossesKilledText != null)
        {
            loseBossesKilledText.text = "Bosses killed: " + bossesKilledThisLevel;
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void IncreaseCurrency(int amount)
    {
        // Increases user currency after each enemy killed
        currency += amount;
        SaveSystem.instance.AddCurrency(amount); // Saves how much money the user has ever made playing the game
    }

    public bool SpendCurrency(int amount)
    {
        // Deduction of currency once bought or upgraded a turret
        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void NextLevel()
    {
        Time.timeScale = 1f; // Reset time

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
        Time.timeScale = 1f; // Reset timer
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        // Closes game (ALt + f4)
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reset timer incase gamehas been paused
        SceneManager.LoadScene("MainMenu");
    }

    internal string WaveTextUI()
    {
        throw new NotImplementedException();
    }

    public IEnumerator ShowErrorTemporarily(float duration)
    {
        NoMoneyText.gameObject.SetActive(true); // actives text
        yield return new WaitForSeconds(duration); // waits X amount of time 
        NoMoneyText.gameObject.SetActive(false); // deactivates text
    }
}