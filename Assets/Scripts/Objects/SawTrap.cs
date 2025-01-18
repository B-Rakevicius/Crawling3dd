using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrap : Trap
{
    public GameObject SawPrefab;
    public int MaxBounces = 2;
    public float SawSpeed = 10f;

    public override void Activate()
    {
        base.Activate();
        if (SawPrefab != null)
        {
            GameObject saw = Instantiate(SawPrefab, transform.position, Quaternion.identity);
            Rigidbody sawRb = saw.GetComponent<Rigidbody>();
            if (sawRb != null)
            {
                sawRb.velocity = transform.forward * SawSpeed;
            }
            Saw sawScript = saw.GetComponent<Saw>();
            if (sawScript != null)
            {
                sawScript.SetBounces(MaxBounces);
            }
        }
    }
}
public class Saw : MonoBehaviour
{
    private int remainingBounces;

    public void SetBounces(int bounces)
    {
        remainingBounces = bounces;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (remainingBounces > 0)
        {
            remainingBounces--;
            // Reflect logic for bouncing
            Vector3 reflectDirection = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
            transform.rotation = Quaternion.LookRotation(reflectDirection);
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = reflectDirection * rb.velocity.magnitude;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}