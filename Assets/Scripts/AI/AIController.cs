using Unity.Burst.CompilerServices;
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
    [SerializeField]
    private GameObject carrySlot;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Collider aiCollider;
    private Vector3 lastTargetPosition;
    private Collider target;
    private BaseStats baseStats;
    private float nextAttackTime;
    private GameObject carriedObject;
    private WaveSpawner spawnPoint;
    private GridPlacementSystem gridPlacementSystem;

    public void SetSpawnPoint(WaveSpawner spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    // Start is called before the first frame update
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        aiCollider = GetComponent<Collider>();
        baseStats = GetComponent<BaseStats>();
        gridPlacementSystem = FindAnyObjectByType<GridPlacementSystem>();
    }

    public void SetLevel(int level)
    {
        baseStats.SetLevel(level);
    }

    // Update is called once per frame
    void Update()
    {
        if (carriedObject == null)
        {
            FindTarget();
        }
        MoveToTarget();
        UpdateAnimator();
        if (isInAttackRange())
        {
            RotateTowardsTarget();
            Attack();
        }
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

    private bool isInAttackRange()
    {
        return GetDistanceToTargetCollider(target) <= baseStats.CurrentStats.attackRange;
    }

    private void RotateTowardsTarget()
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        if (carriedObject != null)
        {
            spawnPoint.LevelUp();
            Destroy(carriedObject);
            carriedObject = null;
        }

        if (Time.time < nextAttackTime)
            return;

        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Use Quaternion.Slerp to smoothly rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        nextAttackTime = Time.time + baseStats.CurrentStats.attackDelay;

        var crumb = target.GetComponent<CrumbFoodSource>();
        if (crumb != null)
        {
            // Pick up crumb logic
            gridPlacementSystem.RemoveObject(target.gameObject, Vector2Int.one);
            target.transform.position = carrySlot.transform.position;
            target.gameObject.transform.parent = transform;
            Destroy(crumb);
            crumb.GetComponent<NavMeshObstacle>().enabled = false;
            crumb.GetComponent<Collider>().enabled = false;

            carriedObject = target.gameObject;
            navMeshAgent.avoidancePriority = 0;
            target = spawnPoint.GetComponent<Collider>();
        }
        else
        {
            // Trigger the attack animation
            animator.SetTrigger("Attack");
            // TODO: Deal Damage on animation callback currentStats.damage
        }
    }

    public void DropCarriedObject()
    {
        if (carriedObject == null)
            return;

        if (!Drop())
        {
            Destroy(target);
        }

        carriedObject = null;
        navMeshAgent.avoidancePriority = 50;
        target = null;
    }

    private bool Drop()
    {
        Vector3? dropPosition = FindNearestDropPosition(carriedObject.transform.position);
        if (dropPosition == null)
            return false;

        // detach carried object
        carriedObject.transform.SetParent(null);
        target.transform.position = dropPosition.Value;

        carriedObject.AddComponent<CrumbFoodSource>();
        carriedObject.GetComponent<NavMeshObstacle>().enabled = true;
        carriedObject.GetComponent<Collider>().enabled = true;
        gridPlacementSystem.AddObject(target.gameObject, Vector2Int.one, 5);

        return true;
    }

    private Vector3? FindNearestDropPosition(Vector3 position)
    {
        int maxDropRadiusX = 5;
        int maxDropRadiusY = 5;

        Vector3Int gridPosition = gridPlacementSystem.WorldToCellPosition(position);
        for (int x = 0; x < maxDropRadiusX; x++)
        {
            for (int y = 0; y < maxDropRadiusY; y++)
            {
                Vector3Int tryPositionGrid = gridPosition + new Vector3Int(x, 0, y);
                Vector3 tryPositionWorld = gridPlacementSystem.CellToWorldPosition(tryPositionGrid);

                bool validOnGrid = gridPlacementSystem.CheckPlacementValidity(tryPositionGrid, Vector2Int.one);
                bool onNavmesh = NavMesh.SamplePosition(tryPositionWorld, out NavMeshHit hit, 0.5f, NavMesh.AllAreas);

                if (validOnGrid && onNavmesh)
                {
                    return gridPlacementSystem.CellToWorldPosition(gridPosition);
                }
            }            
        }
        return null;
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    private void FindTarget()
    {
        target = targetPriority.FindTarget();
    }

    private void MoveToTarget()
    {
        if (target == null || Vector3.Distance(target.transform.position, lastTargetPosition) <= float.Epsilon)
            return;

        navMeshAgent.speed = baseStats.CurrentStats.movementSpeed;

        Vector3 targetPosition = target.transform.position;

        Vector2 randomCircle = Random.insideUnitCircle * spreadRadius;
        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        // Set destination to closest point on the target's collider
        Vector3 destination = target.ClosestPoint(targetPosition + offset);

        NavMeshHit hit;
        NavMesh.SamplePosition(destination, out hit, spreadRadius, NavMesh.AllAreas);

        if (hit.hit)
        {
            navMeshAgent.SetDestination(hit.position);
            lastTargetPosition = target.transform.position;
        }
    }
}
