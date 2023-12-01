using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class RangeIndicator : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 50;
    LineRenderer line;
    private float radius;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        var stats = GetComponentInParent<BaseStats>();
        radius = stats.CurrentStats.attackRange;
        CreatePoints();
    }

    void CreatePoints()
    {
        float x;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, 0.5f, z));

            angle += (360f / segments);
        }
    }
}