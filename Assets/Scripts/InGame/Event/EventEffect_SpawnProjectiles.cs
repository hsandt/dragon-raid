using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event effect: Spawn Projectiles
[AddComponentMenu("Game/Event Effect: Spawn Projectiles")]
public class EventEffect_SpawnProjectiles : MonoBehaviour, IEventEffect
{
    [SerializeField, Tooltip("Serialized Parameters for projectiles to spawn")]
    private ProjectileSpawnSerializedParameters[] spawnSerializedParameters;
        
    public void Trigger()
    {
        foreach (ProjectileSpawnSerializedParameters spawnSerializedParameter in spawnSerializedParameters)
        {
            Vector2 spawnPosition = (Vector2) transform.position + spawnSerializedParameter.relativePosition;
            SpawnProjectile(spawnSerializedParameter.projectilePrefab.name, spawnPosition, spawnSerializedParameter.velocity);
        }

    }

    private void SpawnProjectile(string projectileName, Vector2 position, Vector2 velocity)
    {
        ProjectilePoolManager.Instance.SpawnProjectile(projectileName, position, velocity);
    }
}
