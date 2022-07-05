using System.Collections;
using System.Collections.Generic;
using CommonsDebug;
using UnityEngine;

using UnityConstants;
using CommonsHelper;
using CommonsPattern;
using UnityEngine.Serialization;

/// System that deals damage to an opponent's Health on melee attack hit
/// Collision layers: EnemyMeleeHitBox collides with PlayerHurtBox, PlayerMeleeHitBox colliders with EnemyHurtBox
/// In practice, we don't rely on those collision settings and hardcode Faction => hittable layers
public class MeleeAttack : ClearableBehaviour
{
    // Animator hashes

    private static readonly int meleeAttackHash = Animator.StringToHash("MeleeAttack");

    /// Colliders used internally to store physics 2D query results without allocations
    /// Array length must be big enough to support maximum potential number of targets from a single Melee Attack
    private static readonly Collider2D[] resultColliders = new Collider2D[5];


    [Header("Child component references")]

    [Tooltip("RectContainer describing the Melee Hit Box")]
    public RectContainer meleeHitBox;


    [Header("Parameters data")]

    [Tooltip("Melee Attack Parameters Data")]
    public MeleeAttackParameters meleeAttackParameters;

    [Tooltip("Melee Attack Aesthetics Parameters Data")]
    public MeleeAttackAestheticParameters meleeAttackAestheticParameters;


    [Header("Parameters")]

    [SerializeField, Tooltip("Direction of the hit. It's obvious from the animation but needs to be set as code " +
         "doesn't see what the animation looks like")]
    private HorizontalDirection hitDirection;


    /* Sibling components (required) */

    private Animator m_Animator;
    private CharacterMaster m_CharacterMaster;
    private MeleeAttackIntention m_MeleeAttackIntention;
    public MeleeAttackIntention MeleeAttackIntention => m_MeleeAttackIntention;


    /* Sibling components (optional) */

    private PickUpCollector m_PickUpCollector;


    /* State */

    /// Current state
    private MeleeAttackState m_State;

    /// Current state (getter)
    public MeleeAttackState State => m_State;


    private static int GetOpponentHurtBoxLayerMask(Faction attackerFaction)
    {
        switch (attackerFaction)
        {
            case Faction.Player:
                // Player Melee Attack can destroy both enemies and tangible enemy projectiles
                // It can also be used to catch pick-ups with some extra range
                return Layers.EnemyHurtBoxMask | Layers.EnemyProjectileTangibleMask |
                       Layers.PickUpIntangibleMask | Layers.PickUpTangibleMask;
            case Faction.Enemy:
                // Exceptionally allow "friendly-fire" on enemy projectiles to allow combo
                // like splitting your own rock with a Melee attack.
                // Note that we still don't allow hurting enemy bodies, so it's no true friendly-fire.
                return Layers.PlayerCharacterHurtBoxMask | Layers.EnemyProjectileTangibleMask;
            default:
                Debug.LogErrorFormat("Unknown faction {0}", attackerFaction);
                return 0;
        }
    }

    private void Awake()
    {
        DebugUtil.AssertFormat(meleeHitBox, this,
            "[MeleeAttack] Melee Hit Box not set on Melee Attack component {0}", this);
        DebugUtil.AssertFormat(meleeAttackParameters, this,
            "[MeleeAttack] Melee Attack Parameters not set on Melee Attack component {0}", this);
        DebugUtil.AssertFormat(hitDirection != HorizontalDirection.None, this, "[MeleeAttack] Awake: Hit Direction not set (still None) on {0}", this);

        m_Animator = this.GetComponentOrFail<Animator>();
        m_CharacterMaster = this.GetComponentOrFail<CharacterMaster>();
        m_MeleeAttackIntention = this.GetComponentOrFail<MeleeAttackIntention>();

        m_PickUpCollector = GetComponent<PickUpCollector>();
    }

    public override void Setup()
    {
        m_State = MeleeAttackState.Idle;
    }

    private void FixedUpdate()
    {
        if (ControlUtil.ConsumeBool(ref m_MeleeAttackIntention.startAttack))
        {
            if (CanStartNewAction())
            {
                if (m_State == MeleeAttackState.AttackingCanCancel)
                {
                    // MeleeAttack animation is already playing, so we must force restart
                    // One way is: m_Animator.Play(m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
                    // A simpler way is to Rebind, then let StartAttack set the trigger that will restart the animation
                    m_Animator.Rebind();
                }

                StartAttack();
            }
        }
    }

    /// Return true if character can start a new action from now, whether a Melee Attack or something else
    public bool CanStartNewAction()
    {
        return m_State == MeleeAttackState.Idle || m_State == MeleeAttackState.AttackingCanCancel;
    }

    /// AI usage only: return true is this character is facing target, close enough to hit it with a Melee Attack
    public bool IsCloseEnoughToMeleeAttack(Vector2 targetPosition)
    {
        // Be pragmatic: check if the player character center is inside the melee hit box world rect
        // If this makes the enemy AI too "optimal" and therefore too strong in terms of reactivity, add a bit of margin
        // (shrink test rectangle compared to hit box)
        return meleeHitBox.worldRect.Contains(targetPosition);
    }

    /// Start melee attack with animation and hitbox
    private void StartAttack()
    {
        m_State = MeleeAttackState.Attacking;

        // Animation: play Melee Attack animation
        m_Animator.SetTrigger(meleeAttackHash);

        if (meleeAttackAestheticParameters != null && meleeAttackAestheticParameters.sfxAttack != null)
        {
            // Audio: play attack SFX
            SfxPoolManager.Instance.PlaySfx(meleeAttackAestheticParameters.sfxAttack);
        }
    }

    /// Animation Event callback: apply hitbox damage
    public void MeleeAttackEvent_ApplyHitBoxDamage()
    {
        Faction attackerFaction = m_CharacterMaster.GetFaction();
        int targetLayerMask = GetOpponentHurtBoxLayerMask(attackerFaction);
        int resultsCount = Physics2D.OverlapAreaNonAlloc(meleeHitBox.worldMin, meleeHitBox.worldMax, resultColliders, targetLayerMask);

        for (int i = 0; i < resultsCount; ++i)
        {
            var resultCollider = resultColliders[i];

            // All destructible should have a rigidbody, even if they are not moving (static rigidbody).
            // This is to allow projectile to find the parent owning the HealthSystem.
            Rigidbody2D targetRigidbody = resultCollider.attachedRigidbody;
            if (targetRigidbody != null)
            {
                // `is {}` is enough to check `is not null`, but we added `!= null` for the custom Unity
                // lifetime check (although component is unlikely to be destroyed at this point)
                if (targetRigidbody.GetComponent<PickUp>() is {} targetPickUp && targetPickUp != null)
                {
                    if (m_PickUpCollector != null)
                    {
                        targetPickUp.GetPickedBy(m_PickUpCollector);
                    }
                }
                else if (targetRigidbody.GetComponent<HealthSystem>() is {} targetHealthSystem && targetHealthSystem != null)
                {
                    // Try to damage target found
                    DamageInfo damageInfo = new DamageInfo {
                        damage = meleeAttackParameters.damage,
                        attackerFaction = attackerFaction,
                        elementType = ElementType.Neutral,
                        hitDirection = hitDirection
                    };
                    bool didDamage = targetHealthSystem.TryTakeOneShotDamage(damageInfo);

                    if (didDamage)
                    {
                        if (meleeAttackAestheticParameters != null && meleeAttackAestheticParameters.sfxHit != null)
                        {
                            // Audio: play hit SFX
                            SfxPoolManager.Instance.PlaySfx(meleeAttackAestheticParameters.sfxHit);
                        }
                    }
                }
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                else
                {
                    Debug.LogWarningFormat(resultCollider, "[MeleeAttack] No PickUp nor HealthSystem component found on {0}, " +
                        "yet it contained {1} which was present on targetable layer mask {2}",
                        targetRigidbody, resultCollider, targetLayerMask);
                }
                #endif
            }
        }
    }

    /// Animation Event callback: notify script that character can cancel melee attack with another action from now on
    public void MeleeAttackEvent_NotifyCanCancel()
    {
        m_State = MeleeAttackState.AttackingCanCancel;
    }

    /// Animation Event callback: notify script that character has finished animation without early cancelling
    public void MeleeAttackEvent_NotifyAnimationEnd()
    {
        m_State = MeleeAttackState.Idle;
    }
}
