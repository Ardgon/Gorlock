using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseTowerController : MonoBehaviour
{
    [SerializeField]
    internal float rotationSpeed = 5f;
    [SerializeField]
    internal LayerMask targetLayer;
    [SerializeField]
    internal float projectileSpeed = 15f;
    [SerializeField]
    internal GameObject projectilePrefab;
    [SerializeField]
    internal Transform projectileSpawnPoint;
    [SerializeField]
    internal bool blockRotation;
    [SerializeField]
    internal BaseTargetComponent targetComponent;
    [SerializeField]
    internal bool continuousTargetting;
    [SerializeField]
    internal bool attackCenterMass;
    [SerializeField]
    private AudioSource attackAudio;
    [SerializeField]
    private AudioClip attackClip;

    internal float nextAttackTime;
    internal List<Transform> attackTargets = new();
    internal Transform towerHingeTransform; // Transform to rotate tower model
    internal BaseStats baseStats;
    private Animator animator;

    public void Die()
    {
        var placementSystem = FindObjectOfType<GridPlacementSystem>();
        placementSystem.RemoveTower(this);
        Destroy(gameObject);
    }

    public void LevelUp()
    {
        baseStats.SetLevel(baseStats.CurrentStats.level + 1);
    }

    private void Start()
    {
        towerHingeTransform = GetComponentInChildren<TowerHinge>()?.transform;
        baseStats = GetComponent<BaseStats>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        DetectTarget();
        RotateTowardsTarget();
        PrepareAttack();
    }

    private void PrepareAttack()
    {
        if (IsReadyToAttack())
        {
            animator.SetTrigger("Attack");
            if (attackAudio != null && attackClip != null)
                attackAudio.PlayOneShot(attackClip);
            nextAttackTime = Time.time + baseStats.CurrentStats.attackDelay;
        }
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

    internal float DistanceToTarget(Transform target)
    {
        // Find center of tower
        Vector3 actualTowerPosition = GetComponent<Collider>().bounds.center;
        // Calculate 2D distance
        actualTowerPosition.y = 0f;
        Vector3 targetPosition = new Vector3(target.position.x, 0f, target.position.z);

        return Vector3.Distance(actualTowerPosition, targetPosition);
    }

    internal virtual void DetectTarget()
    {
        if (!continuousTargetting && Time.time < nextAttackTime)
            return;
        
        if (targetComponent == null)
            return;

        attackTargets = targetComponent.DetectTargets(baseStats.CurrentStats.attackRange, targetLayer);
    }

    // Called from animation event
    internal virtual void Attack()
    {
    }

    internal virtual bool IsReadyToAttack()
    {
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the attack range gizmo when the tower is selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, baseStats.CurrentStats.attackRange);
    }
}
