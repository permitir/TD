using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // Key needed to press to instantly go to the Main Menu (SceneIndex = 0)
        {
            MainMenu();
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; //Make sure timer is reset 
        LevelManager.main.LoadMainMenu();
    }   
}
