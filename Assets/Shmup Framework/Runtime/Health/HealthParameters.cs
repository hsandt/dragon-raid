using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Health parameters for Health System
[CreateAssetMenu(fileName = "HealthParameters", menuName = "Data/Health Parameters")]
public class HealthParameters : ScriptableObject
{
    [Tooltip("Initial and max health (health unit)")]
    public int maxHealth = 5;
}
