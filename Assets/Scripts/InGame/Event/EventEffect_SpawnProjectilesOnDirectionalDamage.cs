using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Event effect: Spawn Projectiles on Directional Damage caused by non-neutral entity (may be triggered on Death)
[AddComponentMenu("Game/Event Effect: Spawn Projectiles on Directional Damage")]
public class EventEffect_SpawnProjectilesOnDirectionalDamage : EventEffect_SpawnProjectilesBase, IEventEffectOnDamage
{
    /* IEventEffectOnDamage */

    public void Trigger(DamageInfo damageInfo)
    {
        if (damageInfo.attackerFaction == Faction.None || damageInfo.hitDirection == HorizontalDirection.None)
        {
            // Damage is done by neutral entity or not directional, do nothing
            return;
        }

        foreach (ProjectileSpawnSerializedParameters spawnSerializedParameter in spawnSerializedParameters)
        {
            Vector2 spawnRelativePosition = spawnSerializedParameter.relativePosition;
            Vector2 spawnVelocity = spawnSerializedParameter.velocity;

            // Mirror spawn relative position and velocity if hit direction is Left, as we always define
            // them for a Right hit.
            if (damageInfo.hitDirection == HorizontalDirection.Left)
            {
                spawnRelativePosition.x *= -1f;
                spawnVelocity.x *= -1f;
            }

            ProjectilePoolManager.Instance.SpawnProjectile(
                spawnSerializedParameter.projectilePrefab.name,
                (Vector2) transform.position + spawnRelativePosition,
                spawnVelocity,
                // Associate spawned projectiles to same faction as entity responsible for it
                // (e.g. if Player Character broke a big rock in small rocks, the small rocks are considered
                // a Player attack)
                damageInfo.attackerFaction);
        }
    }
}
