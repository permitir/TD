using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject mainMenu;

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

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); //So I can see if it worked
    }
}
