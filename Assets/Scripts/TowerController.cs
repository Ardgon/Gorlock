using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField]
    private float attackRange = 5f;
    [SerializeField]
    private float rotationSpeed = 5f;
    [SerializeField]
    private LayerMask targetLayer;
    [SerializeField]
    private float attackCooldown = 1f;

    private float nextAttackTime;
    private AIController attackTarget;
    private Transform towerHingeTransform; // Transform to rotate tower model

    public bool blockRotation;

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


    private void Attack()
    {
        if (attackTarget == null
            || Time.time >= nextAttackTime
            || Vector3.Distance(transform.position, attackTarget.transform.position) > attackRange)
            return;

        // Implement attack logic here
        Debug.Log("Attacking " + attackTarget.name);
        nextAttackTime = Time.time + attackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the attack range gizmo when the tower is selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
