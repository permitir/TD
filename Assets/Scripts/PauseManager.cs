using UnityEditor;
using UnityEditor.Embree;
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
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ResumeGame);
        }
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoMainMenu);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
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

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);

            Debug.Log("Game Paused by Player");
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; //reset back to normal time

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
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
