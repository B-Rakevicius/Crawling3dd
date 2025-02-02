using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    public float damage = 20f;
    public float radius = 3f;
    public float lifetime = 0.5f;
    public float knockbackForce = 3f; // Optional knockback

    private void Start()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    enemyAI.TakeDamage((int)damage, knockbackDirection, knockbackForce);
                }
            }
        }

        Destroy(gameObject, lifetime);
    }
}
