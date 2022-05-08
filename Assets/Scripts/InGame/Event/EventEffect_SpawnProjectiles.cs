using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event effect: Spawn Projectiles
[AddComponentMenu("Game/Event Effect: Spawn Projectiles")]
public class EventEffect_SpawnProjectiles : EventEffect_SpawnProjectilesBase, IEventEffect
{
    [Header("Parameters")]

    [SerializeField, Tooltip("Projectile spawn speed")]
    [Min(0f)]
    private float spawnSpeed = 4f;

    [SerializeField, Tooltip("Faction the projectiles should be associated to")]
    private Faction attackerFaction = Faction.Enemy;


    /* IEventEffect */

    public void Trigger()
    {
        foreach (ProjectileSpawnSerializedParameters spawnSerializedParameter in spawnSerializedParameters)
        {
            Vector2 spawnPosition = (Vector2) transform.position + spawnSerializedParameter.relativePosition;
            Vector2 spawnVelocity = spawnSpeed * spawnSerializedParameter.direction.normalized;
            ProjectilePoolManager.Instance.SpawnProjectile(
                spawnSerializedParameter.projectilePrefab.name,
                spawnPosition,
                spawnVelocity,
                attackerFaction);
        }
    }
}
