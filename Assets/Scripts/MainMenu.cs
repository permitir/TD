using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject controls;

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
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

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); //So I can see if it worked
    }
}
