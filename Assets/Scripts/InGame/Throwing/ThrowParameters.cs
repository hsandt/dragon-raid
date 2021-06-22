using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Throw parameters
[CreateAssetMenu(fileName = "ThrowParameters", menuName = "Data/Throw Parameters")]
public class ThrowParameters : ScriptableObject
{
    [Tooltip("Projectile speed (m/s)")]
    public float projectileSpeed = 6f;
}
