using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Throw parameters
[CreateAssetMenu(fileName = "ThrowParameters", menuName = "Data/Throw Parameters")]
public class ThrowParameters : ScriptableObject
{
    [Tooltip("Projectile prefab (must be in Resources/{ProjectilePoolManager.Instance.resourcePrefabsPath})")]
    public GameObject projectilePrefab;
}
