using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    public float SwingSpeed { get; private set; }

    private void Awake()
    {
        WeaponName = "Sword";
        Damage = 25f;
        SwingSpeed = 1.5f;
    }

    public override void Use()
    {
        Debug.Log("Swinging the sword for " + Damage + " damage.");
        // Sword-specific logic (e.g., play animation)
    }
}