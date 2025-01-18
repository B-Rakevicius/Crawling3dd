using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    private Ghost ghostController;

    public void SetGhostController(Ghost ghost)
    {
        ghostController = ghost;
    }

    public override void Die()
    {
        base.Die();

        // Notify the ghost controller
        if (ghostController != null)
        {
            ghostController.ReturnToGhostForm(transform.position); // Respawn ghost at the monster's death position
        }

        Destroy(gameObject);
    }

    public override void Attack()
    {
        // Custom attack logic for the monster
    }
}