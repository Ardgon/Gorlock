using System.Collections.Generic;
using UnityEngine;

public class SingleTargetComponent : BaseTargetComponent
{
    public override List<Transform> DetectTargets(float range, LayerMask targetLayer)
    {
        List<Transform> attackTargets = new List<Transform>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, targetLayer);

        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            // Skip colliders that belong to or are a child of this GameObject
            if (collider.transform.IsChildOf(transform) || collider.transform == transform)
            {
                continue;
            }

            float distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);

            // Check if the current target is closer than the previous nearest target
            if (distanceToTarget < nearestDistance)
            {
                nearestTarget = collider.transform;
                nearestDistance = distanceToTarget;
            }
        }

        if (nearestTarget != null)
        {
            attackTargets.Add(nearestTarget);
        }

        return attackTargets;        
    }
}
