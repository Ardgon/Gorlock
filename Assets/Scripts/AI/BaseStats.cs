using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStats : MonoBehaviour
{
    [SerializeField]
    private string characterType = "basic";
    [SerializeField]
    private StatScaling_SO statScalingDatabase;

    private int level;
    private float movementSpeed;
    private float armor;
    private float maxHealth;
    private float attackSpeed;
    private float damage;

    public float AttackSpeed => attackSpeed;
    public float Damage => damage;
    public float MovementSpeed => movementSpeed;
    public float Armor => armor;
    public float MaxHealth => maxHealth;
}
