using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    public float ArrowSpeed { get; private set; }
    public GameObject ArrowPrefab;

    private void Awake()
    {
        WeaponName = "Bow";
        Damage = 15f;
        ArrowSpeed = 20f;
    }
    public override void Use()
    {
        Debug.Log("Firing an arrow for " + Damage + " damage.");
        // Instantiate and fire an arrow
        if (ArrowPrefab != null)
        {
            GameObject arrow = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
            arrow.GetComponent<Rigidbody>().velocity = transform.forward * ArrowSpeed;
        }
    }
}
