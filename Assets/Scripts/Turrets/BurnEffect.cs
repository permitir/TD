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
        burnDamage = damage;
        burnDuration = duration;
        burnTickRate = tickRate;

        burnCoroutine = StartCoroutine(BurnDamageOverTime());
    }

    private IEnumerator BurnDamageOverTime()
    {
        float elapsed = 0f;
        Health health = GetComponent<Health>();

        while (elapsed < burnDuration && health != null && health.HP > 0)
        {
            yield return new WaitForSeconds(burnTickRate);

            if (health != null && health.HP > 0)
            {
                health.TakeDamage(burnDamage);
                Debug.Log("Damage dealt by burn: " + burnDamage);
            }

            elapsed += burnTickRate;
        }

        // Clear when burn finishes
        burnCoroutine = null;
    }
}
