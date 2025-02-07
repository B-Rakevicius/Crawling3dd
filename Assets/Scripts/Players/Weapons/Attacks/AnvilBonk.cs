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
        StartCoroutine(ApplyShadeAfterDelay(enemies, 1.9f));
        StartCoroutine(DdontShadeAfterDelay(enemies, 2.05f));
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
                    if (enemyRenderer != null)
                    {
                        //Debug.Log("hmmm shading rn");
                        //Transform model = enemy.transform;
                        //ApplyDamageShaderToChildren1(model);
                        //yield return new WaitForSeconds(0.3f);
                        //ApplyDamageShaderToChildren2(model);
                        //ApplyDamageShaderToChildren(model,0.1f);

                    }
                    Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    enemyAI.TakeDamage((int)damage, knockbackDirection, knockbackForce);
                }
            }
        }
        
    }
    private IEnumerator DdontShadeAfterDelay(Collider[] enemies, float delay)
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
                    if (enemyRenderer != null)
                    {
                        Debug.Log("hmmm shading rn");
                        Transform model = enemy.transform;
                        ApplyDamageShaderToChildren2(model);

                    }
                }
            }
        }
    }
    private IEnumerator ApplyShadeAfterDelay(Collider[] enemies, float delay)
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
                    if (enemyRenderer != null)
                    {
                        Debug.Log("hmmm shading rn");
                        Transform model = enemy.transform;
                        ApplyDamageShaderToChildren1(model);
                    }
                }
            }
        }
    }
    private void ApplyDamageShaderToChildren1(Transform model)
    {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            if (damageShader != null)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials.ElementAt(i).shader = damageShader;
                    Debug.Log("Buhblunt0");
                }
            }
             
        }
    }
    private void ApplyDamageShaderToChildren2(Transform model)
    {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            if (damageShader != null)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials.ElementAt(i).shader = normalShader;
                    Debug.Log("Buhblunt1");
                }
            }

        }
    }
}
