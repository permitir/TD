using UnityEngine;

public class FireBullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int initialDamage = 1; // Damage on hit

    [Header("Fire Effect")]
    [SerializeField] private int burnDamage = 1; // Burns per tick
    [SerializeField] private float burnDuration = 2f; // How long enemy burns for
    [SerializeField] private float burnTickRate = 0.5f; // Damage every 0.5 seconds

    private Transform target;

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    private void FixedUpdate()
    {
        //How to track the enemy
        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Take health from enemy
        Health enemyHealth = other.gameObject.GetComponent<Health>();

        if (enemyHealth != null)
        {
            // Initialise damage
            enemyHealth.TakeDamage(initialDamage);

            // Apply the burn
            BurnEffect burnEffect = other.gameObject.GetComponent<BurnEffect>();
            if (burnEffect == null)
            {
                burnEffect = other.gameObject.AddComponent<BurnEffect>();
            }

            burnEffect.ApplyBurn(burnDamage, burnDuration, burnTickRate);
        }

        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
