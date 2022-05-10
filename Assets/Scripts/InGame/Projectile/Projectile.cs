using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsDebug;
using CommonsHelper;
using CommonsPattern;

/// System for Rigidbody2D on projectiles: handles pooling and impact
public class Projectile : MasterBehaviour, IPooledObject
{
    [Header("Parameters data")]

    [Tooltip("Projectile Parameters Data")]
    public ProjectileParameters projectileParameters;

    [Tooltip("Projectile Aesthetic Parameters Data")]
    public ProjectileAestheticParameters projectileAestheticParameters;


    /* Dynamic parameters */

    private Faction m_AttackerFaction;


    /* Sibling components (required) */

    private Rigidbody2D m_Rigidbody2D;


    /* Sibling components (optional) */

    private HealthSystem m_HealthSystem;


    protected override void Init()
    {
        base.Init();

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(projectileParameters, this,
            "[Projectile] Projectile Parameters not set on projectile {0}", this);
        Debug.AssertFormat(projectileAestheticParameters, this,
            "[Projectile] Projectile Aesthetic Parameters not set on projectile {0}", this);
        #endif

        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();

        m_HealthSystem = GetComponent<HealthSystem>();
    }


    /* Collision methods */

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verify that projectile is still alive
        // Of course, on Impact it gets Released, but during the frame of impact, the projectile collider remains active
        // so it can enter multiple colliders at once.
        // It depends on design whether we want to confirm multiple hit in this case.
        // For now, we prefer confirming the first hit only, although we cannot tell which collider will recognize
        // the hit first, because collision is managed by Unity (risk of indeterminism here).
        // The design reason is that if it takes N projectiles to kill 1 enemy of type T,
        // it should take M*N projectiles to kill M enemies of type T, even if they are located at the same place.
        // Of course, if projectiles were considered like AoE bombs, doing multi-hit would make sense.
        if (IsInUse())
        {
            // All destructible should have a rigidbody, even if they are not moving (static rigidbody).
            // This is to allow projectile to find the parent owning the HealthSystem.
            Rigidbody2D targetRigidbody = other.attachedRigidbody;
            if (targetRigidbody != null)
            {
                if (targetRigidbody.GetComponent<HealthSystem>() is {} targetHealthSystem && targetHealthSystem != null)
                {
                    if (targetRigidbody.GetComponent<Projectile>() is { } targetProjectile && targetProjectile != null)
                    {
                        if (m_HealthSystem != null)
                        {
                            // Projectile with health is hitting another projectile with health:
                            // resolve dual collision at once by clearing both projectiles
                            // (we could also just call ReleaseWithImpactFeedback and let the symmetrical
                            // OnTriggerEnter2D do the same on the other side this frame)
                            // If we want to allow simple damage without insta-clear and projectile still crossing,
                            // we should call TryTakeOneShotDamage on both sides (and pass more info like
                            // hit direction to trigger On Death Events properly if needed) and flag the other side
                            // not to call it again;
                            // or only call it on the target, but then we need to store a flag "release at end of frame"
                            // to delay the actual release on Die, so that the other side can enter this block too
                            // thx to IsInUse() being true, to do its job just before disappearing at the end of the frame.
                            ReleaseWithImpactFeedback();
                            targetProjectile.ReleaseWithImpactFeedback();
                        }
                        else
                        {
                            // Projectile without health is hitting projectile with health (generally tangible):
                            // apply normal impact
                            Impact(targetHealthSystem);
                        }
                    }
                    else
                    {
                        // Projectile is hitting a non-projectile with health: apply normal Impact
                        Impact(targetHealthSystem);
                    }
                }
                else if (targetRigidbody.GetComponent<CookedEnemy>() is {} targetCookedEnemy && targetCookedEnemy != null)
                {
                    Impact(targetCookedEnemy);
                }
                else if (targetRigidbody.gameObject.IsInLayerMask(Layers.SolidEnvironmentMask | Layers.DamagingEnvironmentMask))
                {
                    // We cannot damage solid/damaging environment that has no health system, but destroy projectile
                    ReleaseWithImpactFeedback();
                }
            }
        }
    }


    /* Own methods */

    public void WarpAndSetup(Vector2 position, Vector2 velocity, Faction attackerFaction)
    {
        // Experimental hotfix: if you notice relative jittering between projectiles spawned frequently at the same speed,
        // due to pixel perfect camera, use this code to synchronize the sub-pixel at which they are spawned base on
        // current Time
        /*
        float offsetX = velocity.x * Time.time;
        offsetX = offsetX % (1f/16f);
        position.x = Mathf.Round(position.x * 16f) / 16f + offsetX;
        */

        // Set position directly on Transform rather than on Rigidbody2D
        // The latter can work, but only when Spawn is called directly from code, or, if called from an Animation Event,
        // if the Animator Update Mode is set to Animate Physics. Indeed, a Normal mode update will set the rigidbody
        // position *after* the physics update of the frame, and will not update transform position until the next
        // frame, effectively showing the projectile at its incorrect old pooled object position for one frame.
        // We recommend using Animate Physics for Animator that can call physics-related animation events anyway,
        // but in case it is not, to be safe, we set transform position directly.
        transform.position = (Vector3) position;
        m_Rigidbody2D.velocity = velocity;

        m_AttackerFaction = attackerFaction;

        // Set layer dynamically based on attacker faction and tangibility
        // This means that the projectile prefab layer is merely indicative, and always overwritten at runtime
        int layer;

        if (attackerFaction != Faction.None)
        {
            if (attackerFaction == Faction.Player)
            {
                layer = projectileParameters.isTangible ? Layers.PlayerProjectileTangible : Layers.PlayerProjectileIntangible;
            }
            else  // attackerFaction == Faction.Enemy
            {
                layer = projectileParameters.isTangible ? Layers.EnemyProjectileTangible : Layers.EnemyProjectileIntangible;
            }
        }
        else
        {
            DebugUtil.LogErrorFormat("[Projectile] WarpAndSetup: passed attacker faction is None, cannot set layer " +
                "properly, default to EnemyProjectileIntangible");
            layer = Layers.EnemyProjectileIntangible;
        }

        gameObject.layer = layer;

        // Setup can be done before setting transform position, but to follow CharacterMaster we do it after
        // (in case we add components whose Setup rely on master position later)
        Setup();

        if (projectileAestheticParameters != null && projectileAestheticParameters.sfxSpawn != null)
        {
            // Audio: play spawn SFX
            // FIXME: now that we have multi-bullet patterns, SFX stack very fast! Better play them from the Shoot script
            // Actually, bullet hells tend not to play any SFX on minor enemy bullets, so probably
            // only player should play bullet SFX, but remark above still stands
            SfxPoolManager.Instance.PlaySfx(projectileAestheticParameters.sfxSpawn);
        }
    }

    /// Release projectile and play FX/SFX for impact
    /// Note that unlike HealthSystem.Die (if projectile has a Health system, generally tangible projectiles),
    /// it doesn't have side effects like running the On Death Event, so use this when the projectile must be
    /// consumed completely.
    private void ReleaseWithImpactFeedback()
    {
        Release();

        if (projectileAestheticParameters != null)
        {
            // Visual: impact FX appears centered on projectile's last position
            FXPoolManager.Instance.SpawnFX(projectileAestheticParameters.fxImpactName, transform.position);

            if (projectileAestheticParameters.sfxImpact != null)
            {
                // Audio: play impact SFX
                SfxPoolManager.Instance.PlaySfx(projectileAestheticParameters.sfxImpact);
            }
        }
    }

    /// Impact on target health: damage it and self-destruct
    private void Impact(HealthSystem targetHealthSystem)
    {
        // Define damage info
        DamageInfo damageInfo = new DamageInfo
        {
            damage = projectileParameters.damage,
            attackerFaction = m_AttackerFaction,
            elementType = projectileParameters.elementType
            // Ignore hit direction for projectiles, we could use
            // velocity but we'd need to consider relative velocity to screen/target
        };
        bool didDamage = targetHealthSystem.TryTakeOneShotDamage(damageInfo);

        if (didDamage)
        {
            // Our current convention is that projectiles that cannot hit the target goes through it
            // This corresponds to the character being intangible as in traditional shmup post-respawn
            // safety mechanic. In some cases, it may look better to make them block the projectiles
            // (true invincibility), but in this case we'll have to distinguish the types of invincibility
            // (with some enum member) like Smash.

            // Note that we don't call Die, so side effects like On Death Event won't apply.
            // This is wanted, as Impact should just remove the projectile without further effects like spawning
            // sub-projectiles.
            ReleaseWithImpactFeedback();
        }
    }

    /// Impact on target Cooked Enemy: damage it and self-destruct
    private void Impact(CookedEnemy CookedEnemy)
    {
        // Same as HealthSystem, so we could merge both Impact methods but we'll need to make
        // TryTakeOneShotDamage a common interface method
        // FOr now, we keep it separate to allow customizing visual feedback if needed.
        bool didDamage = CookedEnemy.TryTakeOneShotDamage(projectileParameters.damage, projectileParameters.elementType);

        if (didDamage)
        {
            ReleaseWithImpactFeedback();
        }
    }


    /* IPooledObject interface */

    public void Acquire()
    {
        gameObject.SetActive(true);
    }

    public bool IsInUse()
    {
        return gameObject.activeSelf;
    }

    public void Release()
    {
        Clear();
        gameObject.SetActive(false);
    }
}
