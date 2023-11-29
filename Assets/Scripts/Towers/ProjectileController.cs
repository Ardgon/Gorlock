using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private float speed = 15f;
    Transform target;
    private float damageToDeal;
    [SerializeField]
    private float distanceThreshold = 0.1f;

    private Vector3? lastKnownTargetPosition = null;

    public void SetTarget(Transform newTarget, float speed, float damage)
    {
        target = newTarget;
        damageToDeal = damage;
        this.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null && lastKnownTargetPosition == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = target == null ? lastKnownTargetPosition.Value : target.position;

        Vector3 direction = (targetPosition - transform.position).normalized;

        // Move the projectile towards the target
        transform.Translate(direction * speed * Time.deltaTime);
        lastKnownTargetPosition = targetPosition;

        // Check if the projectile is close enough to the target
        if (Vector3.Distance(transform.position, targetPosition) <= distanceThreshold)
        {
            if (target != null)
            {
                target.GetComponent<Health>()?.TakeDamage(damageToDeal);
            }
            Destroy(gameObject);
        }
    }
}
