using UnityEngine;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    public Transform firePoint;
    public LayerMask enemyLayer;

    [System.Serializable]
    public class Attack
    {
        public string attackName;
        public GameObject attackPrefab;
        public AttackType type;
        public float cooldown;
        public float lastUsedTime;
        public KeyCode keybind = KeyCode.None;
    }

    public enum AttackType { Melee, Ranged, PointAndClick, Ranged2, AoeCircle }

    public List<Attack> activeAttacks = new List<Attack>();

    private void Update()
    {
        foreach (var attack in activeAttacks)
        {
            if (attack.keybind != KeyCode.None && Input.GetKeyDown(attack.keybind))
            {
                UseAttack(attack);
            }
        }
    }

    private void UseAttack(Attack attack)
    {
        if (Time.time < attack.lastUsedTime + attack.cooldown) return;

        switch (attack.type)
        {
            case AttackType.Melee:
                Instantiate(attack.attackPrefab, firePoint.position, Quaternion.identity);
                break;

            case AttackType.Ranged:
                Instantiate(attack.attackPrefab, firePoint.position, firePoint.rotation);
                break;
            case AttackType.Ranged2:
                LaunchProjectileAtMouse(attack.attackPrefab);
                break;
            case AttackType.AoeCircle:
                SpawnColdCircle();
                break;
            case AttackType.PointAndClick:
                if (TryPointAndClickAttack(attack.attackPrefab))
                {
                    attack.lastUsedTime = Time.time;
                }
                return;
        }

        attack.lastUsedTime = Time.time;
    }
    private void SpawnColdCircle()
    {
        GameObject coldCircle = new GameObject("ColdCircle");
        coldCircle.transform.position = transform.position;
        coldCircle.AddComponent<ColdCircle>();
    }
    private void LaunchProjectileAtMouse(GameObject projectilePrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, 100f)) // Hit a surface
        {
            targetPosition = hit.point;
        }
        else // No surface hit, shoot in the direction of the ray
        {
            targetPosition = ray.origin + ray.direction * 100f;
        }

        Vector3 direction = (targetPosition - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = direction * 10f; // Adjust speed as needed
        }
    }
    private bool TryPointAndClickAttack3(GameObject abilityPrefab)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Instantiate(abilityPrefab, hit.point, Quaternion.identity);
                return true;
            }
        }
        return false;
    }
    private bool TryPointAndClickAttack(GameObject abilityPrefab)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Transform enemyTransform = hit.collider.transform;
                Bounds enemyBounds = hit.collider.bounds;
                Vector3 spawnPosition = new Vector3(
                    enemyBounds.center.x,
                    enemyBounds.max.y+enemyBounds.size.y*0.5f,
                    enemyBounds.center.z
                );

                GameObject abilityInstance = Instantiate(abilityPrefab, spawnPosition, Quaternion.identity);
                abilityInstance.transform.SetParent(enemyTransform);
                float enemyHeight = enemyBounds.size.y;
                ParticleSystem ps = abilityInstance.GetComponent<ParticleSystem>();
                abilityInstance.transform.localScale = Vector3.one;
                if (ps != null)
                {
                    var main = ps.main;
                    main.startSize = enemyBounds.size.y * 18f;
                    
                    //main.startSizeMultiplier = enemyHeight;
                }

                return true;
            }
        }
        return false;
    }

    public bool AddAttack(Attack newAttack)
    {
        if (activeAttacks.Count < 5)
        {
            activeAttacks.Add(newAttack);
            Debug.Log($"Gained ability: {newAttack.attackName}, Key: {newAttack.keybind}");
            return true;
        }
        else
        {
            Debug.Log("Cannot equip more than 5 abilities.");
            return false;
        }
    }
}
