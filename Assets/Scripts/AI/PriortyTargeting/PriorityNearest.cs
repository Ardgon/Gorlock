using UnityEngine;

public class PriorityNearest : PriorityBase
{
    private Collider aiCollider;

    private void Start()
    {
        aiCollider = GetComponent<Collider>();
    }

    private float GetDistanceToTargetCollider(Collider targetCollider)
    {
        if (targetCollider == null || aiCollider == null)
        {
            Debug.LogError("Tried to find distance to non-existent collider!");
            return float.MaxValue;
        }

        // Calculate the closest points on each collider
        Vector3 closestPointOnFirstCollider = aiCollider.ClosestPoint(targetCollider.bounds.center);
        Vector3 closestPointOnSecondCollider = targetCollider.ClosestPoint(aiCollider.bounds.center);

        Debug.DrawLine(closestPointOnFirstCollider, closestPointOnSecondCollider, Color.red);

        // Calculate the distance between the closest points
        float distance = Vector3.Distance(closestPointOnFirstCollider, closestPointOnSecondCollider);

        return distance;
    }

    public override Collider FindTarget()
    {
        FoodSource[] availableFoodSources = FindObjectsOfType<FoodSource>();

        Collider nearestFoodSource = null;
        float nearestDistance = float.MaxValue;

        foreach (var foodSource in availableFoodSources)
        {
            // Don't target veggies unless blocked
            if (foodSource is VeggieFoodSource)
                continue;

            float distanceToFoodSource = GetDistanceToTargetCollider(foodSource.GetComponent<Collider>());

            if (distanceToFoodSource < nearestDistance)
            {
                nearestFoodSource = foodSource.GetComponent<Collider>();
                nearestDistance = distanceToFoodSource;
            }
        }

        return nearestFoodSource;
    }
}
