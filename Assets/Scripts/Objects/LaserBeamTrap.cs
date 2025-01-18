using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamTrap : Trap
{
    public LineRenderer LaserBeam;
    public float BeamDuration = 2f;

    public override void Activate()
    {
        base.Activate();
        if (LaserBeam != null)
        {
            StartCoroutine(FireLaser());
        }
    }

    private IEnumerator FireLaser()
    {
        LaserBeam.enabled = true;
        Debug.Log("Laser beam firing!");

        yield return new WaitForSeconds(BeamDuration);

        LaserBeam.enabled = false;
        Debug.Log("Laser beam cooldown started.");
    }
}
