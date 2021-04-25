using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Projectile Visual parameters for Projectile
[CreateAssetMenu(fileName = "ProjectileVisualParameters", menuName = "Data/Projectile Visual Parameters")]
public class ProjectileVisualParameters : ScriptableObject
{
    [Tooltip("Name of the FX played on projectile death")]
    public string fxName = "Undefined";
}
