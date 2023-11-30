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
    public float attackDelay;
    [field: SerializeField]
    public float defense;
    [field: SerializeField]
    public float maxHealth;
    [field: SerializeField]
    public float damage;
    [field: SerializeField]
    public float attackRange;
}

[Serializable]
public enum characterType
{
    WorkerAnt, WarriorAnt, ArmoredAnt, ToxicVeganAnt, VeganAnt, BoomerAnt, QueenAnt,
    TomatoTower, LettuceTower, OnionTower, PickelTower, OliveTower, JalapenoTower, FrenchFryTower
}