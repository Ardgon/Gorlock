using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent navmMeshAgent;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        navmMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        FoodSource food = FindObjectOfType<FoodSource>();

        NavMeshHit hit;
        bool hasHit = NavMesh.SamplePosition(food.transform.position, out hit, 2f, NavMesh.AllAreas);

        if (hasHit)
        {
            navmMeshAgent.SetDestination(hit.position);
        }

        animator.SetFloat("Speed", navmMeshAgent.velocity.magnitude);

        if (Vector3.Distance(hit.position, transform.position) < 1f)
        {
            animator.SetTrigger("Attack");
        }
    }
}
