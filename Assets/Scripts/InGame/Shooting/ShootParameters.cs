using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Shoot parameters
[CreateAssetMenu(fileName = "ShootParameters", menuName = "Data/Shoot Parameters")]
public class ShootParameters : ScriptableObject
{
    [Tooltip("Projectile speed (m/s)")]
    public float projectileSpeed = 6f;
    
    [Tooltip("Time required between two Fires (inverse of fire frequency) (s)")]
    public float fireCooldownDuration = 0.5f;
}
