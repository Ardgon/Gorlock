using System;
using System.Collections.Generic;
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
    [SerializeField]
    internal BaseTargetComponent targetComponent;
    [SerializeField]
    internal bool continuousTargetting;
    [SerializeField]
    internal bool attackCenterMass;

    internal float nextAttackTime;
    internal List<Transform> attackTargets = new();
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
        if (attackTargets.Count > 0 && !blockRotation)
        {
            // Rotate towards the first target
            Vector3 targetDirection = attackTargets[0].transform.position - transform.position;
            targetDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            towerHingeTransform.rotation = Quaternion.Slerp(towerHingeTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    internal virtual void DetectTarget()
    {
        if (!continuousTargetting && Time.time < nextAttackTime)
            return;

        attackTargets = targetComponent.DetectTargets(attackRange, targetLayer);
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
