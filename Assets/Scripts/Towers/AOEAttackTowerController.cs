using UnityEngine;

public class AOEAttackTowerController : BaseTowerController
{
    internal override void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, targetLayer);

        foreach (var collider in colliders)
        {
            // Check if the collider has the AIController component
            AIController aiController = collider.GetComponent<AIController>();

            if (aiController != null)
            {
                // Instantiate and shoot a projectile for each enemy in range
                GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                ProjectileController projectileController = projectile.GetComponent<ProjectileController>();

                if (projectileController != null)
                {
                    // Set the target for the projectile
                    projectileController.SetTarget(aiController.transform, projectileSpeed, attackDamage);
                }
            }
        }

        nextAttackTime = Time.time + attackCooldown;
    }
}
