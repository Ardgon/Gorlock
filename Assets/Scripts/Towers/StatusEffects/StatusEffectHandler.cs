using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    [SerializeField]
    private List<BaseStatusEffect> appliedEffects = new List<BaseStatusEffect>();

    public void ApplyEffect(GameObject effectPrefab)
    {
        BaseStatusEffect instantiatedEffect = Instantiate(effectPrefab, transform).GetComponent<BaseStatusEffect>();
        if (instantiatedEffect != null)
        {
            appliedEffects.Add(instantiatedEffect);
        }
        else
        {
            Debug.LogError("Failed to instantiate or cast the effect prefab to BaseStatusEffect.");
        }
    }

    public bool HasEffect(GameObject effectPrefab)
    {
        BaseStatusEffect prefabEffect = effectPrefab.GetComponent<BaseStatusEffect>();
        return prefabEffect != null && appliedEffects.Exists(effect => effect.GetType() == prefabEffect.GetType());
    }

    public void RemoveEffect(GameObject effectPrefab)
    {
        BaseStatusEffect prefabEffect = effectPrefab.GetComponent<BaseStatusEffect>();
        BaseStatusEffect effectToRemove = appliedEffects.Find(effect => effect.GetType() == prefabEffect.GetType());
        if (effectToRemove != null)
        {
            appliedEffects.Remove(effectToRemove);
        }
    }
}
