using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Base class for Event Effect: Spawn Projectiles variants
public abstract class EventEffect_SpawnProjectilesBase : MonoBehaviour
{
    [SerializeField, Tooltip("Serialized Parameters for projectiles to spawn")]
    protected ProjectileSpawnSerializedParameters[] spawnSerializedParameters;
}
