using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    private string saveFilePath;
    private GameData currentData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //Persistent between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Setting the save file path
        saveFilePath = Application.persistentDataPath + "/gamesave.json";
        Debug.Log("Save file location: " + saveFilePath);

        // Load data on start
        LoadGame();
    }

    // saves user data for the game
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved");
    }
    
    // Loads the game
    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            currentData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game loaded! Highest level: " + currentData.highestLevelUnlocked);
        }
        else
        {
            // Creates new save file
            Debug.Log("No save file found, creating new one");
            currentData = new GameData();
            SaveGame();
        }
    }

    //Delete save file (testing purpose)
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted");
            currentData = new GameData();
        }
    }

    // Levels (progress)

    public void UnlockLevel(int levelNumber)
    {
        if (levelNumber > currentData.highestLevelUnlocked)
        {
            currentData.highestLevelUnlocked = levelNumber;
            SaveGame();
            Debug.Log("Level " + levelNumber + " unlocked");
        }
    }

     public bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= currentData.highestLevelUnlocked;
    }

    public int GetHighestLevelUnlocked()
    {
        return currentData.highestLevelUnlocked;
    }

    public void CompleteLevel(int levelNumber)
    {
        //mark level as completed
        if (levelNumber > 0 && levelNumber <= currentData.levelCompleted.Length)
        {
            currentData.levelCompleted[levelNumber - 1] = true;
        }

        // Unlock next level
        UnlockLevel(levelNumber + 1);
        SaveGame();
        
        Debug.Log("Level " + levelNumber + " completed!");
    }

    public bool IsLevelCompleted(int levelNumber)
    {
        if (levelNumber <= 0 || levelNumber > currentData.levelCompleted.Length)
            return false;
        
        return currentData.levelCompleted[levelNumber - 1];
    }

    // audio
    
    public void SetMasterVolume(float volume)
    {
        currentData.masterVolume = volume;
        SaveGame();
    }

    public float GetMasterVolume()
    {
        return currentData.masterVolume;
    }

    public void SetMusicVolume(float volume)
    {
        currentData.musicVolume = volume;
        SaveGame();
    }

    public float GetMusicVolume()
    {
        return currentData.musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        currentData.sfxVolume = volume;
        SaveGame();
    }

    public float GetSFXVolume()
    {
        return currentData.sfxVolume;
    }

    // player/user
    
    public void AddCurrency(int amount)
    {
        currentData.totalCurrency += amount;
        SaveGame();
    }

    public int GetTotalCurrency()
    {
        return currentData.totalCurrency;
    }
}
