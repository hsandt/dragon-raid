using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// Shoot parameters
[CreateAssetMenu(fileName = "ShootParameters", menuName = "Data/Shoot Parameters")]
public class ShootParameters : ScriptableObject
{
    [Tooltip("Projectile speed (m/s)")]
    [FormerlySerializedAs("projectileSpeed")]
    public float holdFireBulletSpeed = 6f;

    [Tooltip("Time required between two shots in continuous fire (inverse of fire frequency) (s). " +
        "Ignored in case of single shot to allow custom bullet pattern definitions on enemies")]
    public float fireCooldownDuration = 0.5f;
}
