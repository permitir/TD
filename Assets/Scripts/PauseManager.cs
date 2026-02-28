using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Paused UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null) // Checking if panel exists
        {
            pausePanel.SetActive(false); // Making sure pause panel is off at start
        }
        if (pauseButton != null) // Checking button panel exists
        {
            pauseButton.onClick.AddListener(PauseGame);
        }
        if (continueButton != null) // Checking if button exists
        {
            continueButton.onClick.AddListener(ResumeGame);
        }
        if (menuButton != null) // Checking if button exists
        {
            menuButton.onClick.AddListener(GoMainMenu);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; //freeze time

        if (pausePanel != null) // Checking if panel exists
        {
            pausePanel.SetActive(true); // Setting the panel to active

            Debug.Log("Game Paused by Player");
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; //reset back to normal time

        if (pausePanel != null) // Checking if panel exists
        {
            pausePanel.SetActive(false); // Disabling panel
            Debug.Log("Game Resumed by Player");
        }
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1f; //Make sure timer is reset 
        LevelManager.main.LoadMainMenu();
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
