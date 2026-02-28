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
            DontDestroyOnLoad(gameObject); // Persistent between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setting the save file path
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
        //Debug.Log("Game saved"); NOTICE: Only ENABLE if testing.
        // IF NOT TESTING, REFRAIN FROM REMOVING THE // as everytime turret is placed or enemy is killed, it will spam logs.
    }
    
    // Loads the game
    public void LoadGame()
    {
        if (File.Exists(saveFilePath)) // check if a save file already exists
        {
            string json = File.ReadAllText(saveFilePath); // Read the save file
            currentData = JsonUtility.FromJson<GameData>(json); // convert JSON into GameData object
            Debug.Log("Game loaded! Highest level: " + currentData.highestLevelUnlocked);
        }
        else
        {
            // Creates new save file
            Debug.LogWarning("No save file found, creating new one"); // warn that no save file exists
            currentData = new GameData(); // create fresh game data with default values
            SaveGame(); // save the new data to create file
        }
    }

    // Delete save file (testing purpose)
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath)) // check if file exists
        {
            File.Delete(saveFilePath); // delete the save file from disk
            Debug.Log("Save file deleted");
            currentData = new GameData(); // reset to fresh game data
        }
    }

    // Levels data (progress)

    public void UnlockLevel(int levelNumber)
    {
        if (levelNumber > currentData.highestLevelUnlocked) // only unlock if this level is higher than current highest
        {
            currentData.highestLevelUnlocked = levelNumber; // update highest unlocked level
            SaveGame(); // save progress to file
            Debug.Log("Level " + levelNumber + " unlocked"); // logging
        }
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= currentData.highestLevelUnlocked; // return true if level number is unlocked. false if not
    }

    public int GetHighestLevelUnlocked()
    {
        return currentData.highestLevelUnlocked; // return highest level unlocked
    }

    public void CompleteLevel(int levelNumber)
    {
        // Will mark level as completed
        if (levelNumber > 0 && levelNumber <= currentData.levelCompleted.Length) // check level number is valid
        {
            currentData.levelCompleted[levelNumber - 1] = true; // mark this level as completed
        }

        // Unlock next level
        UnlockLevel(levelNumber + 1);
        SaveGame();
        
        Debug.Log("Level " + levelNumber + " completed!");
    }

    public bool IsLevelCompleted(int levelNumber)
    {
        if (levelNumber <= 0 || levelNumber > currentData.levelCompleted.Length)
            return false; // return false if invalid level number
        
        return currentData.levelCompleted[levelNumber - 1]; // return completion status
    }

    // Audio data
    
    public void SetMasterVolume(float volume)
    {
        currentData.masterVolume = volume; // update volume setting
        SaveGame(); // save to file
    }

    public float GetMasterVolume()
    {
        return currentData.masterVolume; // return current volume setting
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

    // Player/user data
    
    public void AddCurrency(int amount)
    {
        currentData.totalCurrency += amount;
        SaveGame();
    }

    public int GetTotalCurrency()
    {
        return currentData.totalCurrency;
    }

    public void AddTurretPlaced()
    {
        currentData.totalTurretsPlaced++;
        SaveGame();
    }

    public int GetTotalTurretsPlaced()
    {
        return currentData.totalTurretsPlaced;
    }

    public void AddEnemyKilled()
    {
        currentData.totalEnemiesKilled++;
        SaveGame();
    }

    public int GetTotalEnemiesKilled()
    {
        return currentData.totalEnemiesKilled;
    }
}