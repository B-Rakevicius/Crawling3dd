using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Weapon
{
    public float ManaCost;// { get; protected set; }
    public override void Use()
    {
        Debug.Log("Casting spell: " + WeaponName);
        // Spell-specific logic
    }
}