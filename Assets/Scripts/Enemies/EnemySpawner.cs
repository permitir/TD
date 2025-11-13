using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{

    public static EnemySpawner spawn;

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject bossPrefab;


    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBtwWaves = 5f; //Btw = Between
    [SerializeField] private float difficultyScalingFactor = 0.75f;
    [SerializeField] private float EnemiesPerSecondCap = 15f;

    [Header("Boss Stats")]
    [SerializeField] private int bossCount = 0;
    [SerializeField] private float bossHPMulti = 2f;
    [SerializeField] private int bossWorthIncrease = 100;


    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private float eps; //Enemies Per Second
    private bool isSpawning = false;

    private void Awake()
    {
        spawn = this;
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
        LevelManager.main.WaveTextUI(currentWave);
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (LevelManager.main.isGameOver) return;

        //How enemies are spawned && how waves are ended.
        if (!isSpawning) return;
        timeSinceLastSpawn += Time.deltaTime; //Adds time to counter (seconds)

        if (enemiesLeftToSpawn > 0 && timeSinceLastSpawn >= (1f / Mathf.Max(eps, 0.01f)))
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesAlive <= 0 && enemiesLeftToSpawn <= 0) //Starting a new wave
        {
            EndWave();
        }
    }

    private void EnemyDestroyed()
    {
        //Decreases the amount of enemies alive
        enemiesAlive--;
    }

    private IEnumerator StartWave()
    {
        //What happens once a wave starts
        yield return new WaitForSeconds(timeBtwWaves); //time for user to rest


        Debug.Log("Checking wave:" + currentWave + "vs maxwave:" + LevelManager.main.maxwave); // Checks if condition has been met to win
        if (currentWave >= LevelManager.main.maxwave)
        {
            Debug.Log("Win condition met");
            LevelManager.main.CheckWin(currentWave);
            yield break;
        }

        if (LevelManager.main.isGameOver) yield break;

        if (currentWave % 10 == 0)
        {
            SpawnBoss();
        }
        else
        {
            isSpawning = true;
            enemiesLeftToSpawn = EnemiesPerWave();
            eps = Mathf.Clamp(EnemiesPerSec(), 0.01f, EnemiesPerSecondCap);

            LevelManager.main.WaveTextUI(currentWave); //Updates counter once new wave starts
        }
    }

    private void SpawnBoss()
    {
        currentWave++;
        bossCount++;
        GameObject boss = Instantiate(bossPrefab, LevelManager.main.startPoint.position, Quaternion.identity);
        enemiesAlive++;
        enemiesLeftToSpawn = EnemiesPerWave();
        isSpawning = true;

        Health health = boss.GetComponent<Health>();
        if (health != null)
        {
            health.isBoss = true;

            //multiplies HP by 2 ^ bosscount
            health.HP = Mathf.RoundToInt(health.HP * Mathf.Pow(bossHPMulti, bossCount - 1));

            //increase boss worth
            health.bossWorth += bossWorthIncrease * (bossCount - 1);
        }

        Debug.Log($"Spawned Boss #{bossCount} — HP: {health.HP}, Worth: {health.bossWorth}");

    }

    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;

        LevelManager.main.IncreaseCurrency(25 + (currentWave * 5));

        StartCoroutine(StartWave());
    }

    private void SpawnEnemy()
    {
        //Selects what enemy to spawn every time a new wave starts
        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[index];

        //Selects what enemy to spawn every time a new wave starts
        Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity); 
    }

    private int EnemiesPerWave()
    {
        //How it calculates how many enemies will spawn after each wave ends
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }

    private float EnemiesPerSec()
    {
        return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(currentWave, difficultyScalingFactor), 0f, EnemiesPerSecondCap);
    }

    public int GetWaves()
    {
        return currentWave;
    }

}
