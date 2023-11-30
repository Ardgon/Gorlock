using UnityEngine;

public class AOEAttackTowerController : BaseTowerController
{
    internal override bool IsReadyToAttack()
    {
        return Time.time >= nextAttackTime;
    }
    internal override void Attack()
    {
        base.Attack();

        Collider[] colliders = Physics.OverlapSphere(transform.position, baseStats.CurrentStats.attackRange, targetLayer);

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
                    projectileController.SetTarget(aiController.transform, projectileSpeed, baseStats.CurrentStats.damage, attackCenterMass);
                }
            }
        }

        nextAttackTime = Time.time + baseStats.CurrentStats.attackDelay;
    }
}
