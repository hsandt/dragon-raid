using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

public class Shoot : ClearableBehaviour
{
    [Header("Parameters data")]

    [Tooltip("Shoot Parameters Data")]
    public ShootParameters shootParameters;

    [Tooltip("Enemy Shoot Parameters Data (optional, enemy only)")]
    public EnemyShootParameters enemyShootParameters;


    [Header("Child references")]

    [Tooltip("Transform to spawn projectiles from")]
    public Transform shootAnchor;


    [Header("Parameters")]

    [SerializeField, Tooltip("Default projectile name (must match resource prefab)")]
    private string defaultProjectileName = "Fireball";


    /* Sibling components (required) */

    private CharacterMaster m_CharacterMaster;
    private ShootIntention m_ShootIntention;
    public ShootIntention ShootIntention => m_ShootIntention;


    /* Sibling components (optional) */

    private MeleeAttack m_MeleeAttack;


    /* State */

    /// Time before next Fire is allowed
    private float m_FireCooldownTime;


    private void Awake()
    {
        m_CharacterMaster = this.GetComponentOrFail<CharacterMaster>();
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();

        m_MeleeAttack = GetComponent<MeleeAttack>();
    }

    public override void Setup()
    {
        m_FireCooldownTime = 0f;
    }

    private void FixedUpdate()
    {
        // cool down
        if (m_FireCooldownTime > 0f)
        {
            m_FireCooldownTime = Mathf.Max(0f, m_FireCooldownTime - Time.deltaTime);
        }

        // do not put this in else case of countdown above, in case we've just ended cooldown this frame
        if (m_ShootIntention.holdFire && m_FireCooldownTime <= 0f)
        {
            if (CanShoot())
            {
                // actually fire

                // set cooldown time to prevent firing immediately again
                m_FireCooldownTime = shootParameters.fireCooldownDuration;

                // This time we computed the fire direction right from the actor script, and there can only be one shot
                // on this frame, so unlike the controller, so need to Add it to m_ShootIntention.fireDirections,
                // as we would be consuming and removing it afterward anyway. Instead, compute it as a local variable.
                Vector2 fireDirection;

                // unfortunately, aiming target is for enemies only but we need to check that
                // in case we have enemies that can hold fire
                // maybe this will get tidied up with the introduction of Weapons
                if (enemyShootParameters)
                {
                    fireDirection = GetBaseFireDirection(enemyShootParameters.shootDirectionMode, shootAnchor);
                }
                else
                {
                    // Player only shoots forward
                    fireDirection = shootAnchor.right;
                }

                // spawn projectile with normalized direction and projectile speed
                Vector2 projectileVelocity = shootParameters.projectileSpeed * fireDirection.normalized;
                ProjectilePoolManager.Instance.SpawnProjectile(defaultProjectileName, shootAnchor.position,
                    projectileVelocity, m_CharacterMaster.GetFaction());
            }
        }

        // check for single shot too
        if (ControlUtil.ConsumeBool(ref m_ShootIntention.fireOnce))
        {
            // only allow single shots if not already holding fire
            // however, do not check this above ConsumeBool, as we want to consume the flag every frame,
            // used or not
            if (!m_ShootIntention.holdFire && CanShoot())
            {
                foreach (Vector2 fireDirection in m_ShootIntention.fireDirections)
                {
                    // single shots do not use the fire cooldown time to allow freestyle patterns
                    // otherwise, same principle as the continuous shot
                    Vector2 projectileVelocity = shootParameters.projectileSpeed * fireDirection.normalized;
                    ProjectilePoolManager.Instance.SpawnProjectile(defaultProjectileName, shootAnchor.position,
                        projectileVelocity, m_CharacterMaster.GetFaction());
                }

                m_ShootIntention.fireDirections.Clear();
            }
        }
    }

    /// Return true if the character can shoot now
    private bool CanShoot()
    {
        // Melee attack has priority over shoot
        return m_MeleeAttack == null || m_MeleeAttack.CanStartNewAction();
    }

    /// Return fire direction for given shoot direction mode and shoot anchor
    public static Vector2 GetBaseFireDirection(EnemyShootDirectionMode shootDirectionMode, Transform shootAnchor)
    {
        if (shootDirectionMode == EnemyShootDirectionMode.ShootAnchorForward)
        {
            // Shoot strait using shoot anchor's 2D forward
            // Note that it's generally Left on enemies
            return shootAnchor.right;
        }
        else
        {
            Debug.AssertFormat(shootDirectionMode == EnemyShootDirectionMode.TargetPlayerCharacter,
                "Unsupported EnemyShootDirectionMode {0}, expected EnemyShootDirectionMode.TargetPlayerCharacter as last case",
                shootDirectionMode);

            // Shoot at the player character (normalize now, optional but matches unit vector normalized above)
            // Note that if Player Character is inactive, its last position will be used, as it is a pooled object
            // and therefore not destroyed, just deactivated and staying in place.
            Vector3 playerCharacterPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
            return (Vector2) (playerCharacterPosition - shootAnchor.position).normalized;
        }
    }
}
