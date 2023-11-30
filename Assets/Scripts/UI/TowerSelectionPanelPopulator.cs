using TMPro;
using UnityEngine;

public class TowerSelectionPanelPopulator : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI LevelStat;
    [SerializeField]
    private TextMeshProUGUI HealthStat;
    [SerializeField]
    private TextMeshProUGUI DefenseStat;
    [SerializeField]
    private TextMeshProUGUI AttackDamageStat;
    [SerializeField]
    private TextMeshProUGUI AttackDelayStat;
    [SerializeField]
    private TextMeshProUGUI AttackRangeStat;

    
    public void PopulatePanel(BaseStats stats)
    {
        LevelStat.text = stats.CurrentStats.level.ToString();
        HealthStat.text = stats.CurrentStats.maxHealth.ToString();
        DefenseStat.text = stats.CurrentStats.defense.ToString();
        AttackDamageStat.text = stats.CurrentStats.damage.ToString();
        AttackDelayStat.text = stats.CurrentStats.attackDelay.ToString();
        AttackRangeStat.text = stats.CurrentStats.attackRange.ToString();
    }
}
