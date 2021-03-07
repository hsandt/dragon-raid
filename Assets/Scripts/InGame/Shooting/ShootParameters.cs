using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Shoot parameters
[CreateAssetMenu(fileName = "ShootParameters", menuName = "Data/ShootParameters")]
public class ShootParameters : ScriptableObject
{
    [Tooltip("Projectile speed (m/s)")]
    public float projectileSpeed = 6f;
    
    [Tooltip("Fire frequency (/s)")]
    public float fireFrequency = 2f;
}
