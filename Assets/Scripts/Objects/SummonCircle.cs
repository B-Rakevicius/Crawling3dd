using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class SummonCircle : MonoBehaviour
{
    public void Start()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        
        Ghost ghost = other.GetComponent<Ghost>();
        Debug.Log("TRIGGER ENTERED");
        if (ghost != null)
        {
            Debug.Log("ghost != null");
            ghost.SummonMonster(transform.position);
        }
        else 
        {
            Debug.Log("ghost == null");
        }
    }
}