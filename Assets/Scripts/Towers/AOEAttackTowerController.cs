using UnityEngine;

public class AOEAttackTowerController : BaseTowerController
{
    internal override bool IsReadyToAttack()
    {
        return Time.time >= nextAttackTime && attackTargets.Count > 0;
    }
    internal override void Attack()
    {
        if (attackTargets.Count < 1)
            return;

        base.Attack();

        foreach (Transform target in attackTargets)
        {
            // Check if the collider has the AIController component
            AIController aiController = target.GetComponent<AIController>();

            if (aiController != null)
            {
                // Instantiate and shoot a projectile for each enemy in range
                GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                ProjectileController projectileController = projectile.GetComponent<ProjectileController>();

                if (projectileController != null)
                {
                    // Set the target for the projectile
                    projectileController.SetTarget(aiController.transform, projectileSpeed, baseStats.CurrentStats.damage, attackCenterMass);
                }
            }
        }
    }
}
