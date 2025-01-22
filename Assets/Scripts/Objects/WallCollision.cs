using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, .01f);

        int wallCount = 0;
        GameObject otherWall = null;

        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Wall" && collider.gameObject != this.gameObject)
            {
                wallCount++;
                otherWall = collider.gameObject; // Store the other wall
            }
        }

        // If there are at least two walls (including this one), destroy the other wall
        if (wallCount >= 1) // Since we're excluding this wall, wallCount >= 1 means there's another wall
        {
            Destroy(otherWall); // Destroy the other wall
        }

        GetComponent<Collider>().enabled = true;
    }
}