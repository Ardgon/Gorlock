using UnityEngine;

public class HealTowerController : BaseTowerController
{
    internal override bool IsReadyToAttack()
    {
        return attackTargets.Count == 1
            && Time.time >= nextAttackTime
            && DistanceToTarget(attackTargets[0]) <= baseStats.CurrentStats.attackRange;
    }
    internal override void Attack()
    {
        base.Attack();

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
