using UnityEngine;
public class WaterPlaneFollow : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player's transform
    [SerializeField] private float fixedHeight = 0f; // Fixed height for the water surface

    private void Update()
    {
        if (player != null)
        {
            // Update the plane's position to follow the player
            transform.position = new Vector3(player.position.x, fixedHeight, player.position.z);
        }
    }
}
