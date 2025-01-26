using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Character
{
    public Trap PossessedTrap { get; private set; }
    public List<Monster> MonsterRoster = new List<Monster>(); // A pool of monsters ghosts can summon
    private Monster SummonedMonster;
    public override void Start()
    {
        base.Start();
        // Initialize MonsterRoster or load it dynamically
    }
    // Possess a trap if it's inactive
    public void PossessTrap(Trap trap)
    {
        if (PossessedTrap != null)
        {
            Debug.Log("Already possessing a trap!");
            return;
        }

        if (!trap.IsPossessed)
        {
            PossessedTrap = trap;
            Debug.Log("Ghost has possessed a trap.");
        }
        else
        {
            Debug.Log("Cannot possess an active trap.");
        }
    }
    // Release the currently possessed trap
    public void ReleaseTrap()
    {
        if (PossessedTrap != null)
        {
            PossessedTrap = null;
            Debug.Log("Ghost has released the trap.");
        }
    }
    // Activate the currently possessed trap
    public void ActivateTrap()
    {
        if (PossessedTrap != null)
        {
            PossessedTrap.Activate();
            PossessedTrap = null; // Automatically release possession after activation
            Debug.Log("Trap activated by ghost.");
        }
        else
        {
            Debug.Log("No trap to activate.");
        }
    }
    // Summon a random monster from the ghost's roster
    public void SummonMonster(Vector3 summonCirclePosition)
    {
        if (SummonedMonster != null)
        {
            Debug.Log("Already controlling a monster!");
            return;
        }

        if (MonsterRoster.Count > 0)
        {
            int randomIndex = Random.Range(0, MonsterRoster.Count);
            Monster randomMonsterPrefab = MonsterRoster[randomIndex];
            SummonedMonster = Instantiate(randomMonsterPrefab, summonCirclePosition, Quaternion.identity);
            SummonedMonster.SetGhostController(this); // Link the monster back to the ghost
            Debug.Log($"Summoned monster: {SummonedMonster.name}");
            gameObject.SetActive(false); // Temporarily disable ghost form
        }
        else
        {
            Debug.Log("No monsters in the roster to summon.");
        }
    }
    // Called when the summoned monster dies
    public void ReturnToGhostForm(Vector3 respawnPosition)
    {
        Debug.Log("Returning to ghost form.");
        transform.position = respawnPosition;
        SummonedMonster = null;
        gameObject.SetActive(true); // Reactivate the ghost
    }
    private IEnumerator SlowCoroutine(float duration)
    {
        isSlowed = true;
        Speed *= 0.5f; // Example: Reduce speed by 50%

        yield return new WaitForSeconds(duration);

        Speed = base.originalSpeed;
        isSlowed = false;
    }
    public override void ApplyBurn(float duration)
    {
        base.ApplyBurn(duration);
        Debug.Log($"{name} cannot be burnt because it is a ghost");
    }
    public override void ApplySlow(float duration)
    {
        base.ApplySlow(duration);
        if (!isSlowed)
        {
            StartCoroutine(SlowCoroutine(duration));
        }
    }
    public override void Die()
    {
        Debug.Log($"{name} cannot die because it is a ghost");
        //base.Die();
    }
    public override void Attack()
    {
        base.Attack();
        Debug.Log($"{name} flails their arms around");
        // make ghost pretend to attack
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log($"{name} cannot take damage because it is a ghost");
        // maybe add vfx for taking dmg? cosmetic only
    }
    public override void Move(Vector3 direction)
    {
        base.Move(direction);
        Debug.Log($"{name} is moving according to " + direction + " .");
    }

}