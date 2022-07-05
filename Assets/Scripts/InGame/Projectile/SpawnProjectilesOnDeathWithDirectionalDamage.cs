using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Event effect: Spawn Projectiles on Directional Damage caused by non-neutral entity (may be triggered on Death)
/// Currently copying code from EventEffect_SpawnProjectilesOnDirectionalDamage, latter may be removed at some point.
public class SpawnProjectilesOnDeathWithDirectionalDamage : MonoBehaviour, IDeathHandler
{
    [Header("Parameters")]

    [SerializeField, Tooltip("Serialized Parameters for projectiles to spawn")]
    protected ProjectileSpawnSerializedParameters[] spawnSerializedParameters;

    [Tooltip("Damage range input for linear mapping to projectile spawn speed scaling (clamped beyond this range)")]
    [SerializeField, MinMaxSlider(1f, 10f)]
    private Vector2Int damageInputRange = new(1, 2);

    [Tooltip("Range of projectile spawn speed the damage range input maps to")]
    [SerializeField, MinMaxSlider(0f, 20f)]
    private Vector2 spawnSpeedRange = new(4f, 8f);


    /* IDamageHandler */

    public void OnDeath(DamageInfo lastDamageInfo)
    {
        if (lastDamageInfo.attackerFaction == Faction.None || lastDamageInfo.hitDirection == HorizontalDirection.None)
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
            if (lastDamageInfo.hitDirection == HorizontalDirection.Left)
            {
                spawnRelativePosition.x *= -1f;
                spawnVelocity.x *= -1f;
            }

            // Make spawned projectiles fly faster based on damage
            // Increase velocity from spawnSpeedRange min to max when damage increases from damageInputRange min to max
            // (clamped)
            float spawnSpeed = MathUtil.Remap(damageInputRange.x, damageInputRange.y,
                spawnSpeedRange.x, spawnSpeedRange.y, lastDamageInfo.damage);
            spawnVelocity *= spawnSpeed;

            ProjectilePoolManager.Instance.SpawnProjectile(
                spawnSerializedParameter.projectilePrefab.name,
                (Vector2) transform.position + spawnRelativePosition,
                spawnVelocity,
                // Associate spawned projectiles to same faction as entity responsible for it
                // (e.g. if Player Character broke a big rock in small rocks, the small rocks are considered
                // a Player attack)
                lastDamageInfo.attackerFaction);
        }
    }
}
