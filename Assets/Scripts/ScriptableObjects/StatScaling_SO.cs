using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StatScaling_SO : ScriptableObject
{
    public List<CharacterLevelScaling> characterScaling;
}

[Serializable]
public class CharacterLevelScaling
{
    [field: SerializeField]
    public characterType characterType;
    [field: SerializeField]
    public List<LevelStats> scaling;
}

[Serializable]
public class LevelStats
{
    [field: SerializeField]
    public int level;
    [field: SerializeField]
    public float movementSpeed;
    [field: SerializeField]
    public float attackSpeed;
    [field: SerializeField]
    public float armor;
    [field: SerializeField]
    public float maxHealth;
    [field: SerializeField]
    public float damage;
}

[Serializable]
public enum characterType
{
    WORKER_ANT, WARRIOR_ANT, VEGAN_ANT, TOMATO_TOWER, ONION_TOWER
}