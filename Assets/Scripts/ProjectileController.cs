using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private float speed = 15f;
    Transform target;
    private float damageToDeal;
    [SerializeField]
    private float distanceThreshold = 1f;
    [SerializeField]
    private float destroyDelay = 0.5f;

    public void SetTarget(Transform newTarget, float speed, float damage)
    {
        target = newTarget;
        damageToDeal = damage;
        this.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            Destroy(gameObject);

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        // Move the projectile towards the target
        transform.Translate(direction * speed * Time.deltaTime);

        // Check if the projectile is close enough to the target
        if (Vector3.Distance(transform.position, target.position) <= distanceThreshold)
        {
            target.GetComponent<Health>()?.TakeDamage(damageToDeal);
            Destroy(gameObject, destroyDelay);
        }
    }
}
