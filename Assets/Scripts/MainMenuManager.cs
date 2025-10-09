using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
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
