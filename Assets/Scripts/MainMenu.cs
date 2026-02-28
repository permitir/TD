using System.Collections;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI ErrorLoading;

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

    public void Level2()
    {
        int highestLevel = SaveSystem.instance.GetHighestLevelUnlocked(); // Getting the highest level unlocked as highestLevel

        if (highestLevel >= 2) // If highestLevel unlocked is greated than or equal to 2
        {
            SceneManager.LoadScene("Level 2"); // Load level 2
            Debug.Log("Loading Scene Level 2");
        } 
        else
        {
            if (ErrorLoading != null)
                StartCoroutine(ShowErrorTemporarily(3f)); // shows error for 3 seconds if not unlocked
        }
    }

    public void Level3()
    {
        int highestLevel = SaveSystem.instance.GetHighestLevelUnlocked(); 

        if (highestLevel >= 3)
        {
            SceneManager.LoadScene("Level 3");
            Debug.Log("Loading Scene Level 3");
        } 
        else
        {
            if (ErrorLoading != null)
                StartCoroutine(ShowErrorTemporarily(3f)); // shows error for 3 seconds
        }
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

    private IEnumerator ShowErrorTemporarily(float duration)
    {
        ErrorLoading.gameObject.SetActive(true); // actives text
        yield return new WaitForSeconds(duration); // waits X amount of time 
        ErrorLoading.gameObject.SetActive(false); // deactivates text
    }
}
