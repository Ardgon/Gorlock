using System.Collections.Generic;
using UnityEngine;

public class LowestHealthTargetComponent : BaseTargetComponent
{
    public override List<Transform> DetectTargets(float range, LayerMask targetLayer)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, targetLayer);

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

            float targetHitpoints = targetHealth.GetHitPointPercentage();

            if (targetHitpoints < lowestHealth)
            {
                // Found a new lowest health, clear previous potential targets
                potentialTargets.Clear();
                potentialTargets.Add(targetTransform);
                lowestHealth = targetHitpoints;
            }
            else if (Mathf.Approximately(targetHitpoints, lowestHealth))
            {
                // Add this target to potential targets with the same lowest health
                potentialTargets.Add(targetTransform);
            }
        }

        List<Transform> attackTargets = new();
        if (potentialTargets.Count > 0)
        {
            // Choose a random target from potential targets
            attackTargets.Add(potentialTargets[Random.Range(0, potentialTargets.Count)]);
        }

        return attackTargets;
    }
}
