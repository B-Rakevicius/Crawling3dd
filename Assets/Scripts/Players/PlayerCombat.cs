using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject primaryAttackPrefab; // Left Click
    public GameObject secondaryAttackPrefab; // Q
    public GameObject specialAttackPrefab; // E
    public Transform attackSpawnPoint;
    public float attackCooldown = 0.5f;

    private float nextAttackTime = 0f;

    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0)) // Left Click
            {
                PerformAttack(primaryAttackPrefab);
            }
            else if (Input.GetKeyDown(KeyCode.Q)) // Q Key
            {
                PerformAttack(secondaryAttackPrefab);
            }
            else if (Input.GetKeyDown(KeyCode.E)) // E Key
            {
                PerformAttack(specialAttackPrefab);
            }
        }
    }

    private void PerformAttack(GameObject attackPrefab)
    {
        if (attackPrefab != null)
        {
            Instantiate(attackPrefab, attackSpawnPoint.position, transform.rotation);
            nextAttackTime = Time.time + attackCooldown; // Apply cooldown
        }
    }
}