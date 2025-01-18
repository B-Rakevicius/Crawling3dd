using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    public bool IsHero { get; private set; }
    public Character ControlledCharacter { get; private set; }

    public void BecomeHero(Character hero)
    {
        IsHero = true;
        ControlledCharacter = hero;
    }

    public void BecomeGhost(Character ghost)
    {
        IsHero = false;
        ControlledCharacter = ghost;
    }

    public void Move(Vector3 direction)
    {
        ControlledCharacter?.Move(direction);
    }

    public void SendInput(Vector3 inputDirection)
    {
        Move(inputDirection);
    }
}