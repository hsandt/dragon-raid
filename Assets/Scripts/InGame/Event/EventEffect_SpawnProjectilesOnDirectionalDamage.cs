using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Event effect: Spawn Projectiles on Directional Damage caused by non-neutral entity (may be triggered on Death)
[AddComponentMenu("Game/Event Effect: Spawn Projectiles on Directional Damage")]
public class EventEffect_SpawnProjectilesOnDirectionalDamage : EventEffect_SpawnProjectilesBase, IEventEffectOnDamage
{
    [Header("Parameters")]

    [Tooltip("Damage range input for linear mapping to projectile spawn speed scaling (clamped beyond this range)")]
    [SerializeField, MinMaxSlider(1f, 10f)]
    private Vector2Int damageInputRange = new Vector2Int(1, 2);

    [Tooltip("Range of projectile spawn speed the damage range input maps to")]
    [SerializeField, MinMaxSlider(0f, 20f)]
    private Vector2 spawnSpeedRange = new Vector2(4f, 8f);


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
            Vector2 spawnVelocity = spawnSerializedParameter.direction.normalized;

            // Mirror spawn relative position and velocity if hit direction is Left, as we always define
            // them for a Right hit.
            if (damageInfo.hitDirection == HorizontalDirection.Left)
            {
                spawnRelativePosition.x *= -1f;
                spawnVelocity.x *= -1f;
            }

            // Make spawned projectiles fly faster based on damage
            // Increase velocity from spawnSpeedRange min to max when damage increases from damageInputRange min to max
            // (clamped)
            float spawnSpeed = MathUtil.Remap(damageInputRange.x, damageInputRange.y,
                spawnSpeedRange.x, spawnSpeedRange.y, damageInfo.damage);
            spawnVelocity *= spawnSpeed;

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
