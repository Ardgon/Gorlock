using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeTargetComponent : BaseTargetComponent
{
    public override List<Transform> DetectTargets(float range, LayerMask targetLayer)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, targetLayer);

        List<Transform> attackTargets = new();

        foreach (var collider in colliders)
        {
            Transform targetTransform = collider.transform;

            attackTargets.Add(targetTransform);
        }
       
        return attackTargets;
    }
}
