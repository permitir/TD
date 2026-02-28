using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int bulletDamage = 1;

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
    { //Take health from enemy
        other.gameObject.GetComponent<Health>().TakeDamage(bulletDamage); // Whenever bullet collide with enemy, gets enemy Health script and takes away X health
        Destroy(gameObject); // Destroys bullet
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject); // Whenever bullet leaves scene destroys it
    }

}
