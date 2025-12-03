using System.Collections;
using UnityEngine;

public class StunEffect : MonoBehaviour
{
    private EnemyMovement enemyMovement;
    private bool isStunned = false;
    private Coroutine stunCoroutine;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    public void ApplyStun(float duration)
    {
        // If already stunned, reset the stun timer
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunEnemy(duration));
    }

    private IEnumerator StunEnemy(float duration)
    {
        if (enemyMovement != null && !isStunned)
        {
            isStunned = true;
            
            // Stop the enemy
            enemyMovement.UpdateSpeed(0f);
            GetComponent<SpriteRenderer>().color = Color.yellow;

            // Wait for stun duration
            yield return new WaitForSeconds(duration);

            // Restore movement
            enemyMovement.ResetSpeed();
            GetComponent<SpriteRenderer>().color = Color.blue;
            isStunned = false;
        }
    }

    private void OnDestroy()
    {
        // Clean up coroutine if enemy is destroyed while stunned
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }
    }
}
