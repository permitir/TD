using System.Collections;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class IceTurret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemyMask;

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 3f;
    [SerializeField] private float attackSpeed = 0.5f;
    [SerializeField] private float freezeTime = 1f;

    private float timeUntilFire;

    private void Update()
    {
        timeUntilFire += Time.deltaTime; // increase attack timer each frame

        if (timeUntilFire >= 1f / attackSpeed) // if enough time passed based on attack speed then:
        {
            FreezeEnemiesInRange(); // apply freeze effect on enemies in range
            timeUntilFire = 0f; // resets attack timer
        }
    }

    private void FreezeEnemiesInRange()
    {
        //How it tracks where the enemy is and if it is inside the targeting range of the tower/turret
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask); // detect all enemies within range

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i]; // get currency enemy

                EnemyMovement em = hit.transform.GetComponent<EnemyMovement>(); // em = Enemy Movement (Named it "em" so it doesn't get confused with EnemyMovement script)
                em.UpdateSpeed(0.5f); // slow enemy to 50% of their normal speed

                StartCoroutine(ResetEnemySpeed(em)); // start coroutine to reset speed after duration ends
            }
        }
    }

    private IEnumerator ResetEnemySpeed(EnemyMovement em)
    {
        //Brings the enemy back to its baseSpeed
        yield return new WaitForSeconds(freezeTime);
        em.ResetSpeed();
    }

}
