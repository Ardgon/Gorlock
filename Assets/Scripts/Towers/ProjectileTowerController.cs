using UnityEngine;

public class ProjectileTowerController : BaseTowerController
{
    internal override void Attack()
    {
        if (attackTargets.Count != 1
            || Time.time < nextAttackTime
            || Vector3.Distance(transform.position, attackTargets[0].transform.position) > attackRange)
            return;

        // Instantiate and shoot a projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();

        if (projectileController != null)
        {
            // Set the target for the projectile
            projectileController.SetTarget(attackTargets[0].transform, projectileSpeed, attackDamage, attackCenterMass);
        }

        nextAttackTime = Time.time + attackCooldown;
    }
}
