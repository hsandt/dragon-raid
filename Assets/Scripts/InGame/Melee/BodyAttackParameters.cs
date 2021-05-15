using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Body Attack parameters
[CreateAssetMenu(fileName = "BodyAttackParameters", menuName = "Data/Body Attack Parameters")]
public class BodyAttackParameters : ScriptableObject
{
    [Tooltip("Damage dealt to target on hit (health unit)")]
    public int damage = 1;
}
