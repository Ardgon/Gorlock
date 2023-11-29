using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTargetComponent : MonoBehaviour
{
    public abstract List<Transform> DetectTargets(float range, LayerMask targetLayer);
}
