using System;
using UnityEngine;

public class ProjectileTowerController : BaseTowerController
{
    internal override void Attack()
    {
        if (attackTargets.Count != 1
            || Time.time < nextAttackTime
            || DistanceToTarget(attackTargets[0]) > baseStats.CurrentStats.attackRange)
            return;

        // Instantiate and shoot a projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();

        if (projectileController != null)
        {
            // Set the target for the projectile
            projectileController.SetTarget(attackTargets[0].transform, projectileSpeed, baseStats.CurrentStats.damage, attackCenterMass);
        }

        nextAttackTime = Time.time + baseStats.CurrentStats.attackDelay;
    }
}
