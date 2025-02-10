using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickUpType { point, point10, point100, powerup, misc }
    public PickUpType type;
    public PointManager pointman;
    private void Awake()
    { 
       pointman = GameObject.FindObjectOfType<PointManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("really really realy");
            switch (type)
            {
                case PickUpType.point:
                    pointman.addPoint();
                    Debug.Log(" point  pickup picked up :D ");
                    break;
                case PickUpType.point10:
                    pointman.addPoint(10);
                    Debug.Log(" point X 10 pickup picked up :D ");
                    break;
                case PickUpType.point100:
                    pointman.addPoint(100);
                    Debug.Log(" point X 100 pickup picked up :D ");
                    break;
                case PickUpType.powerup:
                    Debug.Log(" powerup pickup picked up :D ");
                    break;
                case PickUpType.misc:
                    Debug.Log(" misc pickup picked up :D ");
                    return;
            }

            Destroy(gameObject);
        }
    }
}