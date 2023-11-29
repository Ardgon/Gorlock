using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatusEffectData
{
    public GameObject effectPrefab;
    [Range(0f, 1f)]
    public float chanceToApply;
}

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private float distanceThreshold = 0.1f;
    [SerializeField]
    private List<StatusEffectData> statusEffects = new List<StatusEffectData>();

    private float speed = 15f;
    private Transform target;
    private float damageToDeal;
    private bool targetCenterMass = false;
    private Collider targetCollider;
    private Vector3? lastKnownTargetPosition = null;

    public void SetTarget(Transform newTarget, float speed, float damage, bool targetCenterMass = false)
    {
        target = newTarget;
        damageToDeal = damage;
        this.speed = speed;
        this.targetCenterMass = targetCenterMass;
        targetCollider = target?.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ValidateTarget())
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = CalculateTargetPosition();

        MoveTowardsTarget(targetPosition);

        CheckDistanceAndDealDamage(targetPosition);
    }

    private bool ValidateTarget()
    {
        return target != null || lastKnownTargetPosition != null;
    }

    private Vector3 CalculateTargetPosition()
    {
        if (targetCenterMass && target != null && targetCollider != null)
        {
            return targetCollider.bounds.center;
        }
        else
        {
            return target == null ? lastKnownTargetPosition.Value : target.position;
        }
    }

    private void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
        lastKnownTargetPosition = targetPosition;
    }

    private void CheckDistanceAndDealDamage(Vector3 targetPosition)
    {
        if (Vector3.Distance(transform.position, targetPosition) <= distanceThreshold)
        {
            if (target != null)
            {
                target.GetComponent<Health>()?.TakeDamage(damageToDeal);
                ApplyStatusEffects();
            }

            Destroy(gameObject);
        }
    }

    private void ApplyStatusEffects()
    {
        StatusEffectHandler statusEffectHandler = target.GetComponent<StatusEffectHandler>();
        if (statusEffectHandler == null)
        {
            statusEffectHandler = target.gameObject.AddComponent<StatusEffectHandler>();
        }

        foreach (var effectData in statusEffects)
        {
            if (UnityEngine.Random.value <= effectData.chanceToApply && !statusEffectHandler.HasEffect(effectData.effectPrefab))
            {
                statusEffectHandler.ApplyEffect(effectData.effectPrefab);
            }
        }
    }
}
