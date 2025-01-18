using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Trap : MonoBehaviour
{
    public bool IsPossessed { get; private set; } // IsActive is the same as this
    public float ActivationCooldown { get; private set; }
    private bool _isCooldownActive;

    public void Possess(Player ghost)
    {
        IsPossessed = true;
        Debug.Log("Trap possessed by ghost.");
    }
    public virtual void Activate()
    {
        if (!_isCooldownActive)
        {
            Debug.Log("Trap activated!");
            StartCoroutine(StartCooldown());
        }
        else
        {
            Debug.Log("Trap is on cooldown.");
        }
    }
    protected IEnumerator StartCooldown()
    {
        _isCooldownActive = true;
        yield return new WaitForSeconds(ActivationCooldown);
        _isCooldownActive = false;
    }
}

public class TrapProjectile : MonoBehaviour
{
    public float Damage = 10f;
    private float burnDuration;
    private float slowEffect;

    public void ApplyEffects(float burn, float slow)
    {
        burnDuration = burn;
        slowEffect = slow;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Character target = collision.collider.GetComponent<Character>();
        if (target != null)
        {
            target.TakeDamage(Damage);

            // Apply burn effect
            if (burnDuration > 0)
            {
                Debug.Log("Applying burn effect!");
                // Implement burn logic here
            }

            // Apply slow effect
            if (slowEffect > 0)
            {
                Debug.Log("Applying slow effect!");
                // Implement slow logic here
            }
        }

        Destroy(gameObject);
    }
}