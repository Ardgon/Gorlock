using System.Collections.Generic;
using UnityEngine;

public class HealTowerController : BaseTowerController
{
    internal override void DetectTarget()
    {
        if (Time.time < nextAttackTime)
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, targetLayer);

        List<Transform> potentialTargets = new List<Transform>();

        float lowestHealth = float.MaxValue;
        foreach (var collider in colliders)
        {
            Transform targetTransform = collider.transform;

            // Skip colliders that belong to or are a child of this GameObject
            if (targetTransform.IsChildOf(transform) || targetTransform == transform)
            {
                continue;
            }
            var targetHealth = targetTransform.GetComponent<Health>();
            // Check if controller is enabled, i.e. tower is active
            var targetController = targetTransform.GetComponent<BaseTowerController>();
            if (targetHealth == null || targetController == null || targetController.enabled == false)
                continue;

            float targetHitpoints = targetHealth.GetHitHpoint();

            if (targetHitpoints < lowestHealth)
            {
                // Found a new lowest health, clear previous potential targets
                potentialTargets.Clear();
                potentialTargets.Add(targetTransform);
                lowestHealth = targetHitpoints;
            }
            else if (targetHitpoints == lowestHealth)
            {
                // Add this target to potential targets with the same lowest health
                potentialTargets.Add(targetTransform);
            }
        }

        // Choose a random target from potential targets
        attackTarget = potentialTargets.Count > 0 ? potentialTargets[Random.Range(0, potentialTargets.Count)] : null;
    }

    internal override void Attack()
    {
        if (attackTarget == null
            || Time.time < nextAttackTime
            || Vector3.Distance(transform.position, attackTarget.transform.position) > attackRange)
            return;

        // Instantiate and shoot a projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();

        if (projectileController != null)
        {
            // Set the target for the projectile
            projectileController.SetTarget(attackTarget.transform, projectileSpeed, attackDamage, true);
        }

        nextAttackTime = Time.time + attackCooldown;
    }
}
