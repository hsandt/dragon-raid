using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Melee Attack parameters
[CreateAssetMenu(fileName = "MeleeAttackParameters", menuName = "Data/Melee Attack Parameters")]
public class MeleeAttackParameters : ScriptableObject
{
    [Tooltip("Damage dealt to target on hit (health unit)")]
    public int damage = 1;
}
