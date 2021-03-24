using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Health parameters for Health System
[CreateAssetMenu(fileName = "HealthParameters", menuName = "Data/HealthParameters")]
public class HealthParameters : ScriptableObject
{
    [Tooltip("Initial and max health (health unit)")]
    public int maxHealth = 5;
}
