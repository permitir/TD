[System.Serializable]
public class GameData
{
    //Levels
    public int highestLevelUnlocked = 1; // default
    public bool[] levelCompleted = new bool[50]; //track if level completed

    // Audio
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;

    // Stats - for like spotify wrapped but in my game
    public int totalCurrency = 0;
    public int totalEnemiesKilled = 0;
    public int totalTurretsPlaced = 0;

    public GameData()
    {
        highestLevelUnlocked = 1;
        levelCompleted = new bool[50];
        masterVolume = 1f;
        musicVolume = 1f;
        sfxVolume = 1f;
    }
}
