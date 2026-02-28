using System.Collections;
using UnityEngine;

public class BurnEffect : MonoBehaviour
{
    private Coroutine burnCoroutine;
    private int burnDamage;
    private float burnDuration;
    private float burnTickRate;

    public void ApplyBurn(int damage, float duration, float tickRate)
    {
        // Stops the previous burn if it currently exists
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
        }

        // Applies a new burn
        burnDamage = damage; // set damage
        burnDuration = duration; // set duration
        burnTickRate = tickRate; // set tick rate

        burnCoroutine = StartCoroutine(BurnDamageOverTime()); // start burn overtime with coroutine
    }

    private IEnumerator BurnDamageOverTime()
    {
        float elapsed = 0f; // tracks how much time has passed
        Health health = GetComponent<Health>(); // get health from enemy

        while (elapsed < burnDuration && health != null && health.HP > 0) // loop while burn if active and enemy is alive
        {
            yield return new WaitForSeconds(burnTickRate); // wait for tick interval before dealing damage

            if (health != null && health.HP > 0) // checking if enemy is alive
            {
                health.TakeDamage(burnDamage); // deal burn damage
                Debug.Log("Damage dealt by burn: " + burnDamage); // logging
            }

            elapsed += burnTickRate; // increase time elapsed
        }

        // Clear when burn finishes
        burnCoroutine = null;
    }
}
