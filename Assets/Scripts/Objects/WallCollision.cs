using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Collider[] colliders = Physics.OverlapCapsule(this.gameObject.transform.,this.transform.TransformVector,0.01f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.01f);
        //Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(-0.1f,-0.1f,-0.1f));

        int wallCount = 0;
        GameObject otherWall = null;

        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Wall" && collider.gameObject != this.gameObject)
            {
                wallCount++;
                otherWall = collider.gameObject;
            }
        }
        if (wallCount >= 1)
        {
            //Destroy(otherWall);
        }

        GetComponent<Collider>().enabled = true;
    }
}