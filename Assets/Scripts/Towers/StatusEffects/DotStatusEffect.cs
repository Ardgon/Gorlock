using UnityEngine;

public class DotStatusEffect : BaseStatusEffect
{
    [SerializeField]
    private float damagePerTick = 3f;
    [SerializeField]
    private float tickRate = 3f;
    [SerializeField]
    private int ticks = 5;

    private float timeSinceLastTick;
    private Health health;

    internal override void Start()
    {
        base.Start();
        health = GetComponentInParent<Health>();
    }

    private void Update()
    {
        timeSinceLastTick += Time.deltaTime;

        // Calculate the number of ticks that should have occurred during the elapsed time
        int ticksToApply = Mathf.FloorToInt(timeSinceLastTick / (1f / tickRate));

        // Apply damage for each tick
        for (int i = 0; i < ticksToApply; i++)
        {
            health.TakeDamage(damagePerTick);

            ticks--;

            // Check if all ticks are completed
            if (ticks <= 0)
            {
                RemoveStatusEffect();
                return;
            }
        }

        timeSinceLastTick %= 1f / tickRate;
    }
}
