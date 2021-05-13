using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Projectile parameters
[CreateAssetMenu(fileName = "ProjectileParameters", menuName = "Data/Projectile Parameters")]
public class ProjectileParameters : ScriptableObject
{
    [Tooltip("Damage dealt to target on impact (health unit)")]
    public int damage = 1;
}
