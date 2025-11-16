using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelSceneName = "Level 1";

    [Header("UI References")]
    public Button button;
    public TextMeshProUGUI levelNumberText;
    public GameObject lockIcon;
    public GameObject completedCheckmark; // Shows if level is completed

    private void Start()
    {
        UpdateButtonState();
        button.onClick.AddListener(LoadLevel);
    }

    private void UpdateButtonState()
    {
        bool isUnlocked = SaveSystem.instance.IsLevelUnlocked(levelNumber);
        bool isCompleted = SaveSystem.instance.IsLevelCompleted(levelNumber);

        // Enable/disable button
        button.interactable = isUnlocked;

        // Show/hide lock icon
        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);

        // Show checkmark if completed
        if (completedCheckmark != null)
            completedCheckmark.SetActive(isCompleted);

        // Update level number text
        if (levelNumberText != null)
            levelNumberText.text = levelNumber.ToString();
        
        // Optional: Change button color based on state
        ColorBlock colors = button.colors;
        if (isCompleted)
        {
            colors.normalColor = Color.green; // Completed = green
        }
        else if (isUnlocked)
        {
            colors.normalColor = Color.white; // Unlocked = white
        }
        else
        {
            colors.normalColor = Color.gray; // Locked = gray
        }
        button.colors = colors;
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(levelSceneName);
    }
}
