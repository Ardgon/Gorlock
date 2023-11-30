using UnityEngine;

public class Health : MonoBehaviour
{
    private float currentHealth;
    private BaseStats baseStats;

    private void Start()
    {
        baseStats = GetComponent<BaseStats>();
        currentHealth = baseStats.CurrentStats.maxHealth;
    }

    public void TakeDamage(float incomingDamage)
    {
        float actualDamage = incomingDamage - baseStats.CurrentStats.defense;

        // Clamp damage dealt to [1, incomingDamange]
        currentHealth -= Mathf.Clamp(actualDamage, 1f, incomingDamage);

        currentHealth = Mathf.Clamp(currentHealth, 0, baseStats.CurrentStats.maxHealth);
    }

    public void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public float GetHitHpoint() 
    { 
        return currentHealth;
    }

}
