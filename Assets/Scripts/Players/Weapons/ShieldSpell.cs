using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpell : Spell
{
    public float ShieldDuration { get; private set; }

    private void Awake()
    {
        WeaponName = "Shield Spell";
        Damage = 0f;
        ManaCost = 15f;
        ShieldDuration = 10f;
    }
    public override void Use()
    {
        base.Use();
        Debug.Log("Shield spell grants protection for " + ShieldDuration + " seconds.");
        // Shield spell-specific logic
    }
}