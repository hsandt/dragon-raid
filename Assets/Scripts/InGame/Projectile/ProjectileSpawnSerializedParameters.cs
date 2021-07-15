using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileSpawnSerializedParameters
{
    [Tooltip("Projectile prefab (must be in Resources/{ProjectilePoolManager.Instance.resourcePrefabsPath})")]
    public GameObject projectilePrefab;
    
    [Tooltip("Initial position, relative to owner Transform position")]
    public Vector2 relativePosition;
    
    [Tooltip("Initial velocity")]
    public Vector2 velocity;
}
