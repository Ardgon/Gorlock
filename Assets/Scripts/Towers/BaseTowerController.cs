using System;
using UnityEngine;

public class BaseTowerController : MonoBehaviour
{
    [SerializeField]
    internal float attackRange = 5f;
    [SerializeField]
    internal float rotationSpeed = 5f;
    [SerializeField]
    internal LayerMask targetLayer;
    [SerializeField]
    internal float attackCooldown = 1f;
    [SerializeField]
    internal float projectileSpeed = 15f;
    [SerializeField]
    internal GameObject projectilePrefab;
    [SerializeField]
    internal Transform projectileSpawnPoint;
    [SerializeField]
    internal float attackDamage = 1f;
    [SerializeField]
    internal bool blockRotation;

    internal float nextAttackTime;
    internal AIController attackTarget;
    internal Transform towerHingeTransform; // Transform to rotate tower model


    private void Start()
    {
        towerHingeTransform = GetComponentInChildren<TowerHinge>()?.transform;
    }

    private void Update()
    {
        DetectTarget();
        RotateTowardsTarget();
        Attack();
    }

    private void RotateTowardsTarget()
    {
        if (attackTarget != null && !blockRotation)
        {
            // Rotate towards the nearest target
            Vector3 targetDirection = attackTarget.transform.position - transform.position;
            targetDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            towerHingeTransform.rotation = Quaternion.Slerp(towerHingeTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void DetectTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, targetLayer);

        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            // Check if the collider has the AIController component
            AIController aiController = collider.GetComponent<AIController>();

            if (aiController != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);

                // Check if the current target is closer than the previous nearest target
                if (distanceToTarget < nearestDistance)
                {
                    nearestTarget = collider.transform;
                    nearestDistance = distanceToTarget;
                }
            }
        }

        attackTarget = nearestTarget != null ? nearestTarget.GetComponent<AIController>() : null;
    }

    internal virtual void Attack()
    {
        throw new NotImplementedException("Attack should be called on a derived class");
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the attack range gizmo when the tower is selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
