using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3.5f;
    public float stoppingDistance = 1.5f;
    public float rotationSpeed = 5f;
    public float gravity = 10f;
    public LayerMask groundLayer;

    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Damage Feedback")]
    public Renderer enemyRenderer;
    public Color hitColor = Color.red;
    private Color originalColor;
    public float hitFlashDuration = 0.2f;

    [Header("Knockback Settings")]
    public float knockbackResistance = 0.5f; // 0 = no resistance, 1 = full resistance
    private Vector3 knockbackForce;

    [Header("Loot Drop Settings")]
    public GameObject[] commonDrops;  // 60% chance
    public GameObject[] uncommonDrops; // 30% chance
    public GameObject[] rareDrops; // 10% chance
    public float dropChance = 0.5f; // 50% chance to drop an item

    private Transform player;
    private Vector3 velocity;
    private CharacterController controller;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = GetComponent<CharacterController>();
        currentHealth = maxHealth;

        if (controller == null)
        {
            Debug.LogError("CharacterController is missing on enemy!");
        }

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Prevent tilting up/down

        // Apply knockback (gradually reducing effect)
        if (knockbackForce.magnitude > 0.1f)
        {
            controller.Move(knockbackForce * Time.deltaTime);
            knockbackForce = Vector3.Lerp(knockbackForce, Vector3.zero, Time.deltaTime * 5f); // Dampen over time
        }

        // Rotate toward player smoothly
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move toward player
        if (Vector3.Distance(transform.position, player.position) > stoppingDistance)
        {
            controller.Move(direction * speed * Time.deltaTime);
        }

        // Apply gravity
        if (!IsGrounded())
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }

    public void TakeDamage(int damage, Vector3 hitDirection, float knockbackStrength = 0f)
    {
        Debug.Log("ow ow ow shit fuck im taking damage over here " + damage);
        currentHealth -= damage;
        if (knockbackStrength > 0)
        {
            ApplyKnockback(hitDirection, knockbackStrength);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashOnHit());
        }
    }

    private void ApplyKnockback(Vector3 hitDirection, float knockbackStrength)
    {
        Vector3 knockback = hitDirection.normalized * knockbackStrength * (1 - knockbackResistance);
        knockback.y = 0; // Keep it horizontal
        knockbackForce = knockback;
    }

    private void Die()
    {

        ItemManager.Instance.TryDropLoot(transform.position);
        Destroy(gameObject);
    }

    private void DropLoot()
    {
        if (Random.value > dropChance) return; // 50% chance to drop an item

        GameObject[] dropPool;
        float roll = Random.value;

        if (roll < 0.1f) // 10% for rare
            dropPool = rareDrops;
        else if (roll < 0.4f) // 30% for uncommon
            dropPool = uncommonDrops;
        else // 60% for common
            dropPool = commonDrops;

        if (dropPool.Length > 0)
        {
            int index = Random.Range(0, dropPool.Length);
            Instantiate(dropPool[index], transform.position, Quaternion.identity);
        }
    }

    private IEnumerator FlashOnHit()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = hitColor;
            yield return new WaitForSeconds(hitFlashDuration);
            enemyRenderer.material.color = originalColor;
        }
    }
}
