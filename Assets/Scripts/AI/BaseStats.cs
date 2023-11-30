using UnityEngine;

public class BaseStats : MonoBehaviour
{
    [SerializeField]
    private characterType characterType;

    [SerializeField]
    private StatScaling_SO statScalingDatabase;

    private int level = 1;
    private LevelStats currentStats;

    public LevelStats CurrentStats => currentStats;

    public void Awake()
    {
        // TODO: Get this level from wave manager for enemies
        SetLevel(1);
    }

    public void SetLevel(int newLevel)
    {
        level = newLevel;
        currentStats = GwtCurrentStats();
    }

    private LevelStats GwtCurrentStats()
    {
        CharacterLevelScaling characterScaling = GetCharacterScaling();

        if (characterScaling != null)
        {
            LevelStats levelStats = FindStatsForLevel(characterScaling, level);

            if (levelStats != null)
            {
                return levelStats;
            }
            else
            {
                Debug.LogError($"Stats not found for {characterType}.");
            }
        }
        else
        {
            Debug.LogError($"Character scaling not found for {characterType}.");
        }

        return null;
    }

    private CharacterLevelScaling GetCharacterScaling()
    {
        if (statScalingDatabase != null)
        {
            return statScalingDatabase.characterScaling.Find(cs => cs.characterType == characterType);
        }
        else
        {
            Debug.LogError("Stat scaling database not assigned.");
            return null;
        }
    }

    private LevelStats FindStatsForLevel(CharacterLevelScaling characterScaling, int targetLevel)
    {
        // Sort the scaling list by level in ascending order
        characterScaling.scaling.Sort((a, b) => a.level.CompareTo(b.level));

        // Find the first level that is lower than or equal to the target level
        LevelStats levelStats = characterScaling.scaling.Find(stats => stats.level <= targetLevel);

        if (levelStats == null)
        {
            // If no matching or higher level stats are found, use the last (highest) level stats available
            levelStats = characterScaling.scaling[characterScaling.scaling.Count - 1];
        }

        return levelStats;
    }
}
