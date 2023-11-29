using UnityEngine;

public abstract class BaseStatusEffect : MonoBehaviour
{
    //[SerializeField]
    //private Sprite indicatorIcon;
    //[SerializeField]
    //private Image uiIndicator;

    //public Sprite IndicatorIcon => indicatorIcon;

    private StatusEffectHandler statusEffectHandler;

    internal virtual void Start()
    {
        //uiIndicator.sprite = indicatorIcon;
        statusEffectHandler = GetComponentInParent<StatusEffectHandler>(); 
    }
    internal void RemoveStatusEffect()
    {
        statusEffectHandler.RemoveEffect(gameObject);
        Destroy(gameObject);
    }
}
