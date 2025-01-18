using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowProjectileLaserTrap : Trap
{
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 5f;
    public float Cooldown = 2f;
    public float BurnDuration = 0f;
    public float SlowEffect = 0f;

    private bool isReady = true;

    public override void Activate()
    {
        base.Activate();
        if (isReady)
        {
            StartCoroutine(FireProjectile());
        }
    }
    private IEnumerator FireProjectile()
    {
        isReady = false;
        GameObject projectile = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * ProjectileSpeed;
        }

        TrapProjectile trapProjectile = projectile.GetComponent<TrapProjectile>();
        if (trapProjectile != null)
        {
            trapProjectile.ApplyEffects(BurnDuration, SlowEffect);
        }

        yield return new WaitForSeconds(Cooldown);
        isReady = true;
    }
}