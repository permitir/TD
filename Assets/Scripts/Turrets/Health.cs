
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
        get => hp;
        set => hp = value;
    }

    public void TakeDamage(int dmg) //dmg = damage
    {
        hp -= dmg;

        if (hp <= 0)
        {
            if (isBoss)
            {
                LevelManager.main.IncreaseCurrency(bossWorth);
                LevelManager.main.EnemyKilled(isBoss);
            } 
            else
            {
                LevelManager.main.IncreaseCurrency(enemyWorth);
                LevelManager.main.EnemyKilled();
                SaveSystem.instance.AddEnemyKilled();
            }
            
            EnemySpawner.onEnemyDestroy.Invoke();
            Destroy(gameObject);
        }
    }
}
