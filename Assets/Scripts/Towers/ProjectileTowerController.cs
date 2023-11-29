using UnityEngine;

public class ProjectileTowerController : BaseTowerController
{
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
            projectileController.SetTarget(attackTarget.transform, projectileSpeed, attackDamage);
        }

        nextAttackTime = Time.time + attackCooldown;
    }
}