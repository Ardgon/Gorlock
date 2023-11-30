using UnityEngine;

public class PriorityVeggie : PriorityBase
{
    public override Collider FindTarget()
    {
        FoodSource[] availableFoodSources = FindObjectsOfType<FoodSource>();

        Collider nearestFoodSource = null;
        float nearestDistance = float.MaxValue;

        foreach (var foodSource in availableFoodSources)
        {
            float distanceToFoodSource = Vector3.Distance(transform.position, foodSource.transform.position);

            if (foodSource is VeggieFoodSource veggieFoodSource && distanceToFoodSource < nearestDistance)
            {
                nearestFoodSource = veggieFoodSource.GetComponent<Collider>();
                nearestDistance = distanceToFoodSource;
            }
            else if (nearestFoodSource == null && distanceToFoodSource < nearestDistance)
            {
                nearestFoodSource = foodSource.GetComponent<Collider>();
                nearestDistance = distanceToFoodSource;
            }
        }

        return nearestFoodSource;
    }
}
