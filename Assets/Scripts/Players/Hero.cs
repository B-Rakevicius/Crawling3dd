using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Character
{
    public Weapon EquippedWeapon { get; private set; }
    public int Level { get; private set; }
    public int Experience { get; private set; }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical) * Speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }
    public void GainExperience(int amount)
    {
        Experience += amount;
        if (Experience >= Level * 100) // Example leveling system
        {
            LevelUp();
        }
    }
    private void LevelUp()
    {
        Level++;
        CurrentHealth += 10;
        AttackPower += 5;
        Debug.Log("Hero leveled up! Now level " + Level);
    }
    public void EquipWeapon(Weapon weapon)
    {
        EquippedWeapon = weapon;
    }
    private IEnumerator BurnCoroutine(float duration)
    {
        isBurning = true;
        float elapsed = 0f;
        float burnDamage = MaxHealth * 0.05f; // Example: 5% health as burn damage per tick

        while (elapsed < duration)
        {
            elapsed += 1f;
            TakeDamage(burnDamage);
            yield return new WaitForSeconds(1f); // Burn damage every second
        }

        isBurning = false;
    }
    private IEnumerator SlowCoroutine(float duration)
    {
        isSlowed = true;
        Speed *= 0.5f; // Example: Reduce speed by 50%

        yield return new WaitForSeconds(duration);

        Speed = base.originalSpeed;
        isSlowed = false;
    }
    public override void ApplyBurn(float duration)
    {
        base.ApplyBurn(duration);
        if (!isBurning)
        {
            StartCoroutine(BurnCoroutine(duration));
        }
    }
    public override void ApplySlow(float duration)
    {
        base.ApplySlow(duration);
        if (!isSlowed)
        {
            StartCoroutine(SlowCoroutine(duration));
        }
    }
    public override void Die()
    {
        base.Die();
    }
    public override void Attack()
    {
        base.Attack();
        EquippedWeapon?.Use();
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        CurrentHealth -= damage;
        Debug.Log($"Hero took {damage} damage. Current health: {CurrentHealth}");
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public override void Move(Vector3 direction)
    {
        base.Move(direction);
        Debug.Log($"{name} is moving according to " + direction + " .");
    }
}