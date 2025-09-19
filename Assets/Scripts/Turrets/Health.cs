using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int HP = 2; //HP = hitpoints
    [SerializeField] private int enemyWorth = 25;

    public void TakeDamage(int dmg) //dmg = damage
    {
        HP -= dmg;

        if (HP <= 0)
        {
            EnemySpawner.onEnemyDestroy.Invoke();
            LevelManager.main.IncreaseCurrency(enemyWorth); //Player being paid for killing enemy
            Destroy(gameObject);
        }
    }
}
