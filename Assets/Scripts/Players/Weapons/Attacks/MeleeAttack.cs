using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 0.5f;
    public float knockbackForce = 5f; // Adjustable knockback

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy after use
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                enemy.TakeDamage((int)damage, knockbackDirection, knockbackForce);
            }
        }
    }
}
