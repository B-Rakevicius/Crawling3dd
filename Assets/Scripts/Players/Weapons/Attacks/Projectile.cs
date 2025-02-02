using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 15f;
    public float lifetime = 3f;
    public bool applyKnockback = true;
    public float knockbackForce = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                enemy.TakeDamage((int)damage, knockbackDirection, applyKnockback ? knockbackForce : 0f);
            }
            Destroy(gameObject);
        }
    }
}
