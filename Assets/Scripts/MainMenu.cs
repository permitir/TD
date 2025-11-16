using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject enemiesInfo;
    [SerializeField] private GameObject turretsInfo;
    [SerializeField] private GameObject LevelSelect;

    private void Start()
    {
        if (SaveSystem.instance == null)
        {
            GameObject saveSystemObj = new GameObject("SaveSystem");
            saveSystemObj.AddComponent<SaveSystem>();
        }
    }

    public void PlayGame()
    {
        // loads highest level if user played before, if not. Start at 1
        int highestLevel = SaveSystem.instance.GetHighestLevelUnlocked();

        int sceneIndex = highestLevel;

        if (sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
            Debug.Log("Loading Level " + highestLevel);
        }
        else
        {
            // Fallback to Level 1
            SceneManager.LoadScene("Level 1");
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Level 1");
        Debug.Log("Starting new game from Level 1");
    }

    public void OpenLevelSelect()
    {
        mainMenu.SetActive(false);
        LevelSelect.SetActive(true);
    }

    public void HideLevelSelect()
    {
        mainMenu.SetActive(true);
        LevelSelect.SetActive(false);
    }

    public void ShowHowToPlay()
    {
        mainMenu.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void HideHowToPlay()
    {
        mainMenu.SetActive(true);
        howToPlayPanel.SetActive(false);
    }

    public void ShowControls()
    {
        mainMenu.SetActive(false);
        controls.SetActive(true);
    }

    public void HideControls()
    {
        mainMenu.SetActive(true);
        controls.SetActive(false);
    }

    public void ShowEnemiesInfo()
    {
        howToPlayPanel.SetActive(false);
        enemiesInfo.SetActive(true);
    }

    public void HideEnemiesInfo()
    {
        howToPlayPanel.SetActive(true);
        enemiesInfo.SetActive(false);
    }

    public void ShowTurretsInfo()
    {
        howToPlayPanel.SetActive(false);
        turretsInfo.SetActive(true);
    }

    public void HideTurretsInfo()
    {
        howToPlayPanel.SetActive(true);
        turretsInfo.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); //So I can see if it worked
    }
}
