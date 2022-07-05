using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Projectile parameters
[CreateAssetMenu(fileName = "ProjectileParameters", menuName = "Data/Projectile Parameters")]
public class ProjectileParameters : ScriptableObject
{
    [Tooltip("Damage dealt to target on impact (health unit)")]
    public int damage = 1;

    [Tooltip("If true, the projectile is set on a tangible layer, otherwise an intangible layer")]
    public bool isTangible = false;

    [Tooltip("Element type of damage dealt")]
    public ElementType elementType = ElementType.Neutral;
}
