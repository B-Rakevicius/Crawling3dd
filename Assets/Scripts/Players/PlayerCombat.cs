using UnityEngine;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    public Transform firePoint;
    public LayerMask enemyLayer;

    [System.Serializable]
    public class Attack
    {
        public string attackName;
        public GameObject attackPrefab;
        public AttackType type;
        public float cooldown;
        public float lastUsedTime;
        public KeyCode keybind = KeyCode.None;
    }

    public enum AttackType { Melee, Ranged, PointAndClick, Ranged2, AoeCircle }

    public List<Attack> activeAttacks = new List<Attack>();

    private void Update()
    {
        foreach (var attack in activeAttacks)
        {
            if (attack.keybind != KeyCode.None && Input.GetKeyDown(attack.keybind))
            {
                UseAttack(attack);
            }
        }
    }

    private void UseAttack(Attack attack)
    {
        if (Time.time < attack.lastUsedTime + attack.cooldown) return;

        switch (attack.type)
        {
            case AttackType.Melee:
                Instantiate(attack.attackPrefab, firePoint.position, Quaternion.identity);
                break;

            case AttackType.Ranged:
                Instantiate(attack.attackPrefab, firePoint.position, firePoint.rotation);
                break;
            case AttackType.Ranged2:
                LaunchProjectileAtMouse(attack.attackPrefab);
                break;
            case AttackType.AoeCircle:
                SpawnColdCircle();
                break;
            case AttackType.PointAndClick:
                if (TryPointAndClickAttack(attack.attackPrefab))
                {
                    attack.lastUsedTime = Time.time;
                }
                return;
        }

        attack.lastUsedTime = Time.time;
    }
    private void SpawnColdCircle()
    {
       // GameObject ring = new GameObject("ExpandingRing");
       // ring.transform.position = gameObject.transform.position;
       // ring.AddComponent<ExpandingRing>();

        //GameObject ring = new GameObject("ExpandingRingCollider");
        //ring.AddComponent<ExpandingRingCollider>();

        GameObject coldCircle = new GameObject("ColdCircle");
        coldCircle.transform.position = transform.position;
        coldCircle.AddComponent<ColdCircle>();
    }
    private void LaunchProjectileAtMouse(GameObject projectilePrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, 100f)) // Hit a surface
        {
            targetPosition = hit.point;
        }
        else // No surface hit, shoot in the direction of the ray
        {
            targetPosition = ray.origin + ray.direction * 100f;
        }

        Vector3 direction = (targetPosition - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = direction * 10f; // Adjust speed as needed
        }
    }
    private bool TryPointAndClickAttack3(GameObject abilityPrefab)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Instantiate(abilityPrefab, hit.point, Quaternion.identity);
                return true;
            }
        }
        return false;
    }
    private bool TryPointAndClickAttack(GameObject abilityPrefab)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Transform enemyTransform = hit.collider.transform;
                Bounds enemyBounds = hit.collider.bounds;
                Vector3 spawnPosition = new Vector3(
                    enemyBounds.center.x,
                    enemyBounds.max.y+enemyBounds.size.y*0.5f,
                    enemyBounds.center.z
                );

                GameObject abilityInstance = Instantiate(abilityPrefab, spawnPosition, Quaternion.identity);
                abilityInstance.transform.SetParent(enemyTransform);
                float enemyHeight = enemyBounds.size.y;
                ParticleSystem ps = abilityInstance.GetComponent<ParticleSystem>();
                abilityInstance.transform.localScale = Vector3.one;
                if (ps != null)
                {
                    var main = ps.main;
                    main.startSize = enemyBounds.size.y * 18f;
                    
                    //main.startSizeMultiplier = enemyHeight;
                }

                return true;
            }
        }
        return false;
    }

    public bool AddAttack(Attack newAttack)
    {
        if (activeAttacks.Count < 5)
        {
            activeAttacks.Add(newAttack);
            Debug.Log($"Gained ability: {newAttack.attackName}, Key: {newAttack.keybind}");
            return true;
        }
        else
        {
            Debug.Log("Cannot equip more than 5 abilities.");
            return false;
        }
    }
}
public class ExpandingRing : MonoBehaviour
{
    public int segments = 50;
    public float initialRadius = 0.1f;
    public float maxRadius = 5f;
    public float duration = 2f;
    public float lineWidth = 0.1f;
    public float damage = 15f;
    public float knockbackForce = 0f;
    private LineRenderer lineRenderer;
    private SphereCollider ringCollider;
    private float elapsedTime = 0f;

    void Start()
    {
        // Line Renderer setup
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;

        // Collider setup (Squashed)
        ringCollider = gameObject.AddComponent<SphereCollider>();
        ringCollider.isTrigger = true;
        ringCollider.radius = initialRadius;
        ringCollider.center = new Vector3(0,0,0);

        UpdateRing(initialRadius);
    }
    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easedRadius = Mathf.Lerp(initialRadius, maxRadius, EaseOutQuad(t));

            UpdateRing(easedRadius);
            ringCollider.radius = easedRadius; // Expand collider
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void UpdateRing(float radius)
    {
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * (2 * Mathf.PI / segments);
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            lineRenderer.SetPosition(i, transform.position + pos);
        }
    }
    float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Modify to suit your needs
        {
            Debug.Log("Hit: " + other.name);
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                Renderer enemyRenderer = other.GetComponent<Renderer>();
                if (enemyRenderer != null)
                {
                    // if renderer maybe render 
                }
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                enemyAI.TakeDamage((int)damage, knockbackDirection, knockbackForce);
            }
        }
    }
}


public class ExpandingRingSprite : MonoBehaviour
{
    public float expansionSpeed = 3f;
    public float maxSize = 5f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("RingSprite"); // Replace with your ring sprite
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f); // Semi-transparent
        transform.localScale = Vector3.one * 0.5f; // Start small
    }

    void Update()
    {
        if (transform.localScale.x < maxSize)
        {
            transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class ExpandingRingCollider : MonoBehaviour
{
    public float expansionSpeed = 3f;
    public float maxSize = 5f;
    public int damage = 5;
    public int knockbackForce = 10;

    private SphereCollider sphereCollider;

    void Start()
    {
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 0.1f; // Start small
    }

    void Update()
    {
        if (sphereCollider.radius < maxSize)
        {
            sphereCollider.radius += expansionSpeed * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage, knockbackDir, knockbackForce);
            }
        }
    }
}
