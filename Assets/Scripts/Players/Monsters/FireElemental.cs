using UnityEngine;

public class FireElemental : Monster
{
    public float BurnDuration = 5f;

    public override void Start()
    {
        base.Start();
        MaxHealth = 100f;
        CurrentHealth = MaxHealth;
        AttackPower = 20f;
        Speed = 4f;
    }

    public override void Attack()
    {
        base.Attack();
        Debug.Log($"{name} launches a fireball!");

        // Example: Fireball projectile
        GameObject fireball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fireball.transform.position = transform.position + transform.forward;
        Rigidbody rb = fireball.AddComponent<Rigidbody>();
        rb.velocity = transform.forward * 10f; // Launch the fireball forward
        fireball.AddComponent<Fireball>(); // Add custom script for fireball behavior
        Destroy(fireball, 3f); // Destroy fireball after 3 seconds
    }

    public override void Move(Vector3 direction)
    {
        base.Move(direction);
        transform.Translate(direction.normalized * Speed * Time.deltaTime);
    }
}

public class Fireball : MonoBehaviour
{
    public float Damage = 20f;
    public float BurnDuration = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        Character target = collision.collider.GetComponent<Character>();
        if (target != null)
        {
            target.TakeDamage(Damage);
            target.ApplyBurn(BurnDuration);
            Debug.Log($"{target.name} hit by fireball and is burning for {BurnDuration} seconds!");
        }

        Destroy(gameObject); // Destroy fireball on impact
    }
}
