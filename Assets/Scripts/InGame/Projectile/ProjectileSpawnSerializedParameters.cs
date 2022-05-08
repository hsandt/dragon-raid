using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileSpawnSerializedParameters
{
    [Tooltip("Projectile prefab (must be in Resources/{ProjectilePoolManager.Instance.resourcePrefabsPath})")]
    public GameObject projectilePrefab;

    [Tooltip("Initial position, relative to owner Transform position. If this is used by an Event Effect " +
        "on Damage, define it for a hit in the Right direction.")]
    public Vector2 relativePosition;

    [Tooltip("Initial velocity. If this is used by an Event Effect " +
        "on Damage, define it for a hit in the Right direction.")]
    public Vector2 velocity;
}
