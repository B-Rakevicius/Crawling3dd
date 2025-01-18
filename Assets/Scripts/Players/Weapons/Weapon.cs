using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string WeaponName { get; protected set; }
    public float Damage { get; protected set; }
    public abstract void Use();
}