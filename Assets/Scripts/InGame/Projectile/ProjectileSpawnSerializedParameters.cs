using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileSpawnSerializedParameters
{
    [Tooltip("Projectile prefab (must be in Resources/{ProjectilePoolManager.Instance.resourcePrefabsPath})")]
    public GameObject projectilePrefab;
    
    [Tooltip("Initial position")]
    public Vector2 position;
    
    [Tooltip("Initial velocity")]
    public Vector2 velocity;
}
