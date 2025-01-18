using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLaserTrap : Trap
{
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 20f;
    public float FireRate = 0.2f;
    public float BurnDuration = 0f;
    public float SlowEffect = 0f;
    private Coroutine firingRoutine;
    /* burn
    public float FireRate = 0.2f;
    public float BurnDuration = 3f;
    public float SlowEffect = 0f;
    */
    /* slow
    public float FireRate = 0.2f;
    public float BurnDuration = 0f;
    public float SlowEffect = 1.5f;
    */
    public override void Activate()
    {
        base.Activate();
        if (ProjectilePrefab != null && firingRoutine == null)
        {
            firingRoutine = StartCoroutine(FireProjectiles());
        }
    }

    private IEnumerator FireProjectiles()
    {
        while (true)
        {
            GameObject projectile = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = transform.forward * ProjectileSpeed;
            }

            // Apply status effects
            TrapProjectile trapProjectile = projectile.GetComponent<TrapProjectile>();
            if (trapProjectile != null)
            {
                trapProjectile.ApplyEffects(BurnDuration, SlowEffect);
            }

            yield return new WaitForSeconds(FireRate);
        }
    }
}
