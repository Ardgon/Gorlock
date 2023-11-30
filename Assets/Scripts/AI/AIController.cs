using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private float spreadRadius = 1f;
    [SerializeField]
    private float rotationSpeed = 1f;
    [SerializeField]
    private PriorityBase targetPriority;

    private NavMeshAgent navmMeshAgent;
    private Animator animator;
    private Collider aiCollider;
    private Vector3 lastTargetPosition;
    private Collider target;
    private BaseStats baseStats;
    private float nextAttackTime;

    // Start is called before the first frame update
    void Awake()
    {
        navmMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        aiCollider = GetComponent<Collider>();
        baseStats = GetComponent<BaseStats>();
    }

    public void SetLevel(int level)
    {
        baseStats.SetLevel(level);
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
        MoveToTarget();
        UpdateAnimator();
        Attack();
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

        // Calculate the distance between the closest points
        float distance = Vector3.Distance(closestPointOnFirstCollider, closestPointOnSecondCollider);

        return distance;
    }

    private void Attack()
    {
        if (Time.time < nextAttackTime || (GetDistanceToTargetCollider(target) > baseStats.CurrentStats.attackRange))
            return;

        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Use Quaternion.Slerp to smoothly rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Trigger the attack animation
        animator.SetTrigger("Attack");
        // TODO: Deal Damage on animation callback currentStats.damage
        nextAttackTime = Time.time + baseStats.CurrentStats.attackDelay;
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", navmMeshAgent.velocity.magnitude);
    }

    private void FindTarget()
    {
        target = targetPriority.FindTarget();
    }

    private void MoveToTarget()
    {
        if (target == null || Vector3.Distance(target.transform.position, lastTargetPosition) <= float.Epsilon)
            return;

        navmMeshAgent.speed = baseStats.CurrentStats.movementSpeed;

        Vector3 targetPosition = target.transform.position;

        Vector2 randomCircle = Random.insideUnitCircle * spreadRadius;
        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        Vector3 destination = targetPosition + offset;

        NavMeshHit hit;
        NavMesh.SamplePosition(destination, out hit, spreadRadius, NavMesh.AllAreas);

        if (hit.hit)
        {
            navmMeshAgent.SetDestination(hit.position);
            lastTargetPosition = target.transform.position;
        }
    }
}
