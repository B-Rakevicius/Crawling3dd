using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingLaserTrap : Trap
{
    public LineRenderer LaserBeam;
    public float Damage = 50f;
    public float StatusDuration = 3f; // For burn or slow effects
    public bool ApplyBurn = false;
    public bool ApplySlow = false;
    public float BeamDuration = 1f; // Duration the beam stays active

    private List<GameObject> hitTargets = new List<GameObject>();
    private bool isFiring = false;

    public override void Activate()
    {
        base.Activate();

        if (!isFiring)
        {
            StartCoroutine(FireLaser());
        }
    }

    private IEnumerator FireLaser()
    {
        isFiring = true;
        LaserBeam.enabled = true;
        hitTargets.Clear();

        float elapsedTime = 0f;

        while (elapsedTime < BeamDuration)
        {
            elapsedTime += Time.deltaTime;

            // Raycast logic to detect targets
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                LaserBeam.SetPosition(0, transform.position);
                LaserBeam.SetPosition(1, hit.point);

                GameObject target = hit.collider.gameObject;

                // Ensure each target is hit only once during the beam's active duration
                if (!hitTargets.Contains(target))
                {
                    hitTargets.Add(target);

                    Character character = target.GetComponent<Character>();
                    if (character != null)
                    {
                        character.TakeDamage(Damage);

                        // Apply burn effect
                        if (ApplyBurn)
                        {
                            character.ApplyBurn(StatusDuration);
                        }

                        // Apply slow effect
                        if (ApplySlow)
                        {
                            character.ApplySlow(StatusDuration);
                        }
                    }
                }
            }
            else
            {
                // Set laser endpoint to max distance if no hit
                LaserBeam.SetPosition(0, transform.position);
                LaserBeam.SetPosition(1, transform.position + transform.forward * 100f);
            }

            yield return null;
        }

        LaserBeam.enabled = false;
        isFiring = false;
    }
}