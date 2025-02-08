using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
public class AnvilBonk : MonoBehaviour
{
    public float damage = 20f;
    public float radius = 3f;
    public float lifetime = 2.4f;
    public float knockbackForce = 3f;
    public Shader damageShader;
    public Shader normalShader;
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private void Start()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, radius);
        StartCoroutine(ApplyDamageAfterDelay(enemies, 1.9f));
        Destroy(gameObject, lifetime);
    }
    private IEnumerator ApplyDamageAfterDelay(Collider[] enemies, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Collider enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    Renderer enemyRenderer = enemy.GetComponent<Renderer>();
                    Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    enemyAI.TakeDamage((int)damage, knockbackDirection, knockbackForce);
                }
            }
        }
        
    }
}
