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
    [SerializeField]
    private AudioSource attackAudio;

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
    private float targetSearchCooldown = 5f;
    private float timeSinceLastSearch = 0f;

    public float speedModifier = 0f;
    public Coroutine speedModifierCoroutine;

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
        if (carriedObject == null && timeSinceLastSearch <= 0)
        {
            FindTarget();
            timeSinceLastSearch = targetSearchCooldown;
        }
        MoveToTarget();
        UpdateAnimator();
        if (isInAttackRange())
        {
            RotateTowardsTarget();
            Attack();
        }

        timeSinceLastSearch -= Time.deltaTime;
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

    // Called from animation
    private void Hit()
    {
        if (target == null)
            return;

        target.GetComponent<Health>()?.TakeDamage(baseStats.CurrentStats.damage);
        if (attackAudio != null)
            attackAudio.Play();
    }

    public void Die()
    {
        DropCarriedObject();
        GameMode.Instance.AddCoins(2);
        Destroy(gameObject);
    }

    public void DropCarriedObject()
    {
        if (carriedObject == null)
            return;

        if (!Drop())
        {
            Destroy(carriedObject);
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
        carriedObject.transform.position = dropPosition.Value;

        carriedObject.AddComponent<CrumbFoodSource>();
        carriedObject.GetComponent<NavMeshObstacle>().enabled = true;
        carriedObject.GetComponent<Collider>().enabled = true;
        gridPlacementSystem.AddObject(carriedObject.gameObject, Vector2Int.one, 7);

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

                bool validOnGrid = gridPlacementSystem.CheckPlacementValidity(tryPositionGrid, Vector2Int.one, 0);
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


        if (target != null)
        {
            Vector3 closestPointOnTargetCollider = target.ClosestPoint(aiCollider.bounds.center);
            bool hasHit = NavMesh.SamplePosition(closestPointOnTargetCollider, out NavMeshHit hit, 1.5f, NavMesh.AllAreas);

            // No valid point on navmesh to walk to
            if (!hasHit)
            {
                // If the path is not complete, find the nearest reachable FoodSource
                FindNearestReachableFoodSource();
                return;
            }

            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(hit.position, path);

            // Path broken by obstacle
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                // If the path is not complete, find the nearest reachable FoodSource
                FindNearestReachableFoodSource();
            }

            // Food can be reached
        }
    }

    private void FindNearestReachableFoodSource()
    {
        FoodSource[] foodSources = FindObjectsOfType<FoodSource>();
        float minDistance = float.MaxValue;
        FoodSource nearestFoodSource = null;

        foreach (var foodSource in foodSources)
        {
            Collider foodCollider = foodSource.GetComponent<Collider>();
            float distance = GetDistanceToTargetCollider(foodCollider);

            // Check if the FoodSource is reachable
            if (IsFoodSourceReachable(foodCollider))
            {
                // Update nearest reachable food source if the current one is closer
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestFoodSource = foodSource;
                }
            }
        }

        if (nearestFoodSource != null)
        {
            // Set the nearest reachable FoodSource as the new target
            target = nearestFoodSource.GetComponent<Collider>();
        } else
        {
            Debug.LogWarning("Found no available food source but path is blocked");
        }
    }

    private bool IsFoodSourceReachable(Collider foodSourceCollider)
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 closestPointOnTargetCollider = aiCollider.ClosestPoint(foodSourceCollider.bounds.center);

        navMeshAgent.CalculatePath(closestPointOnTargetCollider, path);

        // Check if the path to the FoodSource is complete
        return path.status == NavMeshPathStatus.PathComplete;
    }


    private void MoveToTarget()
    {
        if (target == null || Vector3.Distance(target.transform.position, lastTargetPosition) <= float.Epsilon)
            return;

        navMeshAgent.speed = baseStats.CurrentStats.movementSpeed + speedModifier;

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
