using UnityEngine;

public class PriorityNearest : PriorityBase
{
    public override Collider FindTarget()
    {
        FoodSource[] availableFoodSources = FindObjectsOfType<FoodSource>();

        Collider nearestFoodSource = null;
        float nearestDistance = float.MaxValue;

        foreach (var foodSource in availableFoodSources)
        {
            float distanceToFoodSource = Vector3.Distance(transform.position, foodSource.transform.position);

            if (distanceToFoodSource < nearestDistance)
            {
                nearestFoodSource = foodSource.GetComponent<Collider>();
                nearestDistance = distanceToFoodSource;
            }
        }

        return nearestFoodSource;
    }
}
