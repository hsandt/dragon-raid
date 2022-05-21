using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Damaging Environment parameters
[CreateAssetMenu(fileName = "DamagingEnvironmentParameters", menuName = "Data/Damaging Environment Parameters")]
public class DamagingEnvironmentParameters : ScriptableObject
{
    [Tooltip("Damage dealt to entity colliding with the damaging environment (health unit)")]
    public int damage = 1;
}
