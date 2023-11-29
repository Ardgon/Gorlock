using UnityEngine;

public class HealTowerController : ProjectileTowerController
{
    internal override void DetectTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, targetLayer);

        Transform target = null;
        float lowestHealth = float.MaxValue;

        foreach (var collider in colliders)
        {
            // Skip colliders that belong to or are a child of this GameObject
            if (collider.transform.IsChildOf(transform) || collider.transform == transform)
            {
                continue;
            }

            var targetHealth = collider.GetComponent<Health>();
            if (targetHealth == null)
                continue;

            // Check if the current target is closer than the previous nearest target
            if (targetHealth.GetHitHpoint() < lowestHealth)
            {
                target = collider.transform;
                lowestHealth = targetHealth.GetHitHpoint();
            }
        }

        attackTarget = target != null ? target : null;
    }
}
