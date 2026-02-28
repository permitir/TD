using Unity.Mathematics;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int hp = 2; //HP = hitpoints
    [SerializeField] private int enemyWorth = 25;

    [Header("Boss")]
    public bool isBoss = false; // Checks if the wave is for a boss
    public int bossWorth = 250; 

    public int HP
    {
        get => hp; // allows other script to read current HP
        set => hp = value; // allows other scripts to modify the hp
    }

    public void TakeDamage(int dmg) //dmg = damage
    {
        hp -= dmg; // subtract damage from current health

        if (hp <= 0)
        {
            if (isBoss)
            {
                LevelManager.main.IncreaseCurrency(bossWorth); // give player boss reward currency
                LevelManager.main.EnemyKilled(isBoss); // track boss kill in stats
            } 
            else
            {
                LevelManager.main.IncreaseCurrency(enemyWorth); // give player enemy worth reward currency
                LevelManager.main.EnemyKilled(); // track enemy kill in stats
                SaveSystem.instance.AddEnemyKilled(); // save total enemy killed lifetime
            }
            
            EnemySpawner.onEnemyDestroy.Invoke(); // notify spawner that enemy died
            Destroy(gameObject); // destroy enemy from scene
        }
    }
}
