using UnityEngine;
public class AgileHunter : Monster
{
    public override void Start()
    {
        base.Start();
        MaxHealth = 120f;
        CurrentHealth = MaxHealth;
        AttackPower = 15f;
        Speed = 7f;
    }
    public override void Attack()
    {
        base.Attack();
        Debug.Log($"{name} performs a quick dash attack!");

        // Example: Dash forward and deal damage
        Vector3 dashDirection = transform.forward * 3f; // Dash 3 units forward
        transform.position += dashDirection;

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 1.5f); // 1.5-unit radius
        foreach (Collider enemy in hitEnemies)
        {
            Character target = enemy.GetComponent<Character>();
            if (target != null)
            {
                target.TakeDamage(AttackPower);
            }
        }
    }
    public override void Move(Vector3 direction)
    {
        base.Move(direction);
        transform.Translate(direction.normalized * Speed * Time.deltaTime);
    }
}
