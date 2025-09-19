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


    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBtwWaves = 5f; //Btw = Between
    [SerializeField] private float difficultyScalingFactor = 0.75f;
    [SerializeField] private float EnemiesPerSecondCap = 15f;


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

        if (timeSinceLastSpawn >= (1f / eps) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesAlive == 0 && enemiesLeftToSpawn == 0) //Starting a new wave
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

        if (currentWave > LevelManager.main.maxwave)
        {
            LevelManager.main.CheckWin(currentWave);
            yield break;
        }

        if (LevelManager.main.isGameOver) yield break;

        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
        eps = EnemiesPerSec();

        LevelManager.main.WaveTextUI(currentWave); //Updates counter once new wave starts
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
