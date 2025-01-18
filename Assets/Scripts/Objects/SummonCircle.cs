using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SummonCircle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ghost ghost = other.GetComponent<Ghost>();
        if (ghost != null)
        {
            ghost.SummonMonster(transform.position);
        }
    }
}