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
        float actualDamage = incomingDamage;
        if (incomingDamage >= 0)
        {
            actualDamage -= baseStats.CurrentStats.defense;
            actualDamage = Mathf.Clamp(actualDamage, 1f, incomingDamage);
        }

        // Clamp damage dealt to [1, incomingDamange]
        currentHealth -= actualDamage;

        currentHealth = Mathf.Clamp(currentHealth, 0, baseStats.CurrentStats.maxHealth);
    }

    public void Update()
    {
        if (currentHealth <= 0)
        {
            var aiController = GetComponent<AIController>();
            if (aiController != null)
            {
                aiController.DropCarriedObject();
            }
            Destroy(gameObject);
        }
    }

    public float GetHitPointPercentage() 
    { 
        return currentHealth / baseStats.CurrentStats.maxHealth;
    }

}
