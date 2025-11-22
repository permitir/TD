[System.Serializable]
public class GameData
{
    //Levels
    public int highestLevelUnlocked = 1; // Default
    public bool[] levelCompleted = new bool[3]; // Tracks if level completed

    // Audio
    public float masterVolume = 1f; // Default value
    public float musicVolume = 1f; // Default value
    public float sfxVolume = 1f; // Default value

    // Stats - for like spotify wrapped but in my game
    public int totalCurrency = 0;
    public int totalEnemiesKilled = 0;
    public int totalTurretsPlaced = 0;

    public GameData()
    {
        highestLevelUnlocked = 1;
        levelCompleted = new bool[3];
        masterVolume = 1f;
        musicVolume = 1f;
        sfxVolume = 1f;
    }
}
