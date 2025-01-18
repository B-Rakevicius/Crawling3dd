using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    public float Speed = 5f;
    public float originalSpeed;

    public float MaxHealth = 100f;
    public float CurrentHealth;

    public float AttackPower;

    public bool isBurning = false;
    public bool isSlowed = false;
    public virtual void Start()
    {
        CurrentHealth = MaxHealth;
        originalSpeed = Speed;
    }



    public virtual void Attack()
    {
        Debug.Log($"{name} is trying to attack.");
    }
    public virtual void TakeDamage(float damage)
    {
        Debug.Log($"{name} is going to take damage.");
    }
    public virtual void Die()
    {
        // Handle character death (e.g., respawn, remove from party)
        Debug.Log($"{name} has died.");
        Destroy(gameObject);
    }
    public virtual void Move(Vector3 direction)
    {
        Debug.Log($"{name} is going to move.");
    }
    public virtual void ApplyBurn(float duration)
    {
        Debug.Log($"{name} is going to be burnt.");
    }
    public virtual void ApplySlow(float duration)
    {
        Debug.Log($"{name} is going to be slowed.");
    }
}