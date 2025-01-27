using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempMiner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    /*
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0)) // Left-click to mine
        {
            Vector3 worldPosition = GetMouseWorldPosition();
            Vector2Int chunkCoord = GetChunkCoord(worldPosition);
            Chunk chunk = chunks[chunkCoord];

            // Convert world position to local position in the chunk
            Vector3 localPosition = chunk.transform.InverseTransformPoint(worldPosition);

            // Start mining (remove voxels in a small radius, e.g., 1.0f)
            chunk.ModifyVoxel(localPosition, 1.0f, -1.0f); // Negative value for digging
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point; // Return the world position of the clicked block
        }
        return Vector3.zero;
    }
    public ParticleSystem miningEffect;

    private void PlayMiningEffect(Vector3 position)
    {
        ParticleSystem effect = Instantiate(miningEffect, position, Quaternion.identity);
        effect.Play();
    }
    */
}
