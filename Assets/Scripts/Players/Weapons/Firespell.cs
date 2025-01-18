using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : Spell
{
    public float BurnDuration { get; private set; }

    private void Awake()
    {
        WeaponName = "Fire Spell";
        Damage = 40f;
        ManaCost = 20f;
        BurnDuration = 5f;
    }

    public override void Use()
    {
        base.Use();
        Debug.Log("Fire spell burns enemies for " + BurnDuration + " seconds.");
        // Fire spell-specific logic
    }
}