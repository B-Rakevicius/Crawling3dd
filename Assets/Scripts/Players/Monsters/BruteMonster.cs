using UnityEngine;
public class BruteMonster : Monster
{
    public override void Start()
    {
        base.Start();
        MaxHealth = 200f;
        CurrentHealth = MaxHealth;
        AttackPower = 30f;
        Speed = 3f;
    }
    public override void Attack()
    {
        base.Attack();
        Debug.Log($"{name} performs a heavy smash attack!");

        // Example: Melee attack in an area
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 2f); // 2-unit radius
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
