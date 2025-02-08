using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColdCircle : MonoBehaviour
{
    public ParticleSystem coldParticles;
    public ParticleSystem windParticles;
    public int damage = 2;
    public int knockbackForce = 0;

    public SphereCollider sphereCol;
    public int burstCount = 300;
    public float initialRadius = 0.1f;
    private float elapsedTime = 0f;
    public float duration = 2f;
    public float maxRadius = 5f;
    void Start()
    {
        if (coldParticles == null)
        {
            coldParticles = CreateColdParticles();
        }
        if (windParticles == null)
        {
            windParticles = CreateWindParticles();
        }
        if (sphereCol == null)
        {
            sphereCol = gameObject.AddComponent<SphereCollider>();
        }
        sphereCol.isTrigger = true;
        sphereCol.radius = initialRadius;
        EmitParticles();
        StartCoroutine(ExpandEffect());
    }

    void EmitParticles()
    {
        if (coldParticles != null)
        {
            coldParticles.Emit(burstCount);
        }
        if (windParticles != null)
        {
            windParticles.Emit(burstCount / 2);
        }
    }
    IEnumerator ExpandEffect()
    {
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easedT = EaseOutQuad(t);

            // Expand the sphere collider radius
            sphereCol.radius = Mathf.Lerp(initialRadius, maxRadius, easedT);

            // Adjust particle systems
            UpdateParticleSize(easedT);

            yield return null;
        }

        Destroy(gameObject);
    }
    private void UpdateParticleSize(float easedT)
    {
        var coldShape = coldParticles.shape;
        coldShape.radius = Mathf.Lerp(0.5f, maxRadius, easedT);
        var windShape = windParticles.shape;
        windShape.radius = Mathf.Lerp(0.7f, maxRadius, easedT);
    }
    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

        }
        else
        {
            Destroy(gameObject);
        }
    }
    float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) 
        {
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                Renderer enemyRenderer = other.GetComponent<Renderer>();
                Vector3 knockbackDirection = (enemyAI.transform.position - transform.position).normalized;
                enemyAI.TakeDamage((int)damage, knockbackDirection, knockbackForce);
            }
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = other.transform.position - transform.position;
                direction.y = 0;
                rb.AddForce(direction.normalized * knockbackForce, ForceMode.Impulse);
            }
        }
    }
    private ParticleSystem CreateColdParticles()
    {
        GameObject particleObj = new GameObject("ColdParticles");
        particleObj.transform.position = transform.position;
        particleObj.transform.parent = transform;

        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psRenderer = particleObj.GetComponent<ParticleSystemRenderer>();
        psRenderer.material = new Material(Shader.Find("Standard"));

        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(Color.cyan);
        main.startSize = 0.2f;
        main.startLifetime = 1.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = initialRadius;
        shape.rotation = new Vector3(90, 0, 0);

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.radial = 1.5f;

        return ps;
    }
    private ParticleSystem CreateWindParticles()
    {
        GameObject particleObj = new GameObject("WindParticles");
        particleObj.transform.position = transform.position;
        particleObj.transform.parent = transform;

        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psRenderer = particleObj.GetComponent<ParticleSystemRenderer>();
        psRenderer.material = new Material(Shader.Find("Standard"));

        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.8f, 0.8f, 1f, 0.5f));
        main.startSize = 0.3f;
        main.startLifetime = 1.8f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = initialRadius;
        shape.rotation = new Vector3(90, 0, 0);

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.radial = 2f;
        
        return ps;
    }
}