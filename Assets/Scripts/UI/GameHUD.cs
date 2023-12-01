using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI healthDisplay;

    [SerializeField]
    TextMeshProUGUI coinsDisplay;

    // Update is called once per frame
    void Update()
    {
        healthDisplay.text = GameMode.Instance.GetHitPoints().ToString();
        coinsDisplay.text = GameMode.Instance.GetCoins().ToString();
    }
}
