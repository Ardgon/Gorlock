using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseStatusEffect : MonoBehaviour
{
    //[SerializeField]
    //private Sprite indicatorIcon;
    //[SerializeField]
    //private Image uiIndicator;

    //public Sprite IndicatorIcon => indicatorIcon;

    internal virtual void Start()
    {
        //uiIndicator.sprite = indicatorIcon;
    }
}
