using System.Collections;
using System.Collections.Generic;
using UnityConstants;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System that deals damage to an opponent's Health on melee attack hit
/// Collision layers: EnemyMeleeHitBox collides with PlayerHurtBox, PlayerMeleeHitBox colliders with EnemyHurtBox
/// In practice, we don't rely on those collision settings and hardcode Faction => hittable layers
public class MeleeAttack : ClearableBehaviour
{
    /// Internal state enum
    private enum State
    {
        /// Not attacking, no animation (another script may be doing another action)
        Idle,
        /// Attacking, cannot cancel yet
        Attacking,
        /// Attacking and finishing the animation, can cancel any time
        AttackingCanCancel
    }
    
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
    public MeleeAttackParameters bodyAttackParameters;
    
    [Tooltip("Melee Attack Visual Parameters Data")]
    public MeleeAttackAestheticParameters bodyAttackAestheticParameters;
    
    
    [Header("Parameters")]

    [SerializeField, Tooltip("Faction the attacker belongs to")]
    private Faction m_AttackerFaction;
    
    
    /* Sibling components */
    
    private Animator m_Animator;
    private MeleeAttackIntention m_MeleeAttackIntention;

    
    /* State */

    /// Current state
    private State m_State;


    private void Awake()
    {
        Debug.AssertFormat(meleeHitBox, this,
            "[MeleeAttack] Melee Hit Box not set on Melee Attack component {0}", this);
        
        Debug.AssertFormat(bodyAttackParameters, this,
            "[MeleeAttack] Melee Attack Parameters not set on Melee Attack component {0}", this);
        
        m_Animator = this.GetComponentOrFail<Animator>();
        m_MeleeAttackIntention = this.GetComponentOrFail<MeleeAttackIntention>();
    }

    public override void Setup()
    {
        m_State = State.Idle;
    }

    private static int GetOpponentHurtBoxLayerMask(Faction faction)
    {
        switch (faction)
        {
            case Faction.Player:
                return Layers.EnemyHurtBoxMask;
            case Faction.Enemy:
                return Layers.PlayerCharacterHurtBoxMask;
            default:
                Debug.LogErrorFormat("Unknown faction {0}", faction);
                return 0;
        }
    }

    private void FixedUpdate()
    {
        if (ControlUtil.ConsumeBool(ref m_MeleeAttackIntention.startAttack))
        {
            switch (m_State)
            {
                case State.Idle:
                    StartAttack();
                    break;
                case State.AttackingCanCancel:
                    // MeleeAttack animation is already playing, so we musts force restart
                    // One way is: m_Animator.Play(m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
                    // A simpler way is to Rebind, then let StartAttack set the trigger that will restart the animation
                    m_Animator.Rebind();
                    StartAttack();
                    break;
            }
        }
    }

    /// Start melee attack with animation and hitbox
    private void StartAttack()
    {
        m_State = State.Attacking;
        
        // Animation: play Melee Attack animation
        m_Animator.SetTrigger(meleeAttackHash);
        
        if (bodyAttackAestheticParameters != null && bodyAttackAestheticParameters.sfxAttack != null)
        {
            // Audio: play attack SFX
            SfxPoolManager.Instance.PlaySfx(bodyAttackAestheticParameters.sfxAttack);
        }
    }

    /// Event callback: apply hitbox damage
    public void ApplyHitBoxDamage()
    {
        int targetLayerMask = GetOpponentHurtBoxLayerMask(m_AttackerFaction);
        int resultsCount = Physics2D.OverlapAreaNonAlloc(meleeHitBox.worldMin, meleeHitBox.worldMax, resultColliders, targetLayerMask);

        for (int i = 0; i < resultsCount; ++i)
        {
            var resultCollider = resultColliders[i];
            
            // All destructible should have a rigidbody, even if they are not moving (static rigidbody).
            // This is to allow projectile to find the parent owning the HealthSystem.
            Rigidbody2D targetRigidbody = resultCollider.attachedRigidbody;
            if (targetRigidbody != null)
            {
                var targetHealthSystem = targetRigidbody.GetComponent<HealthSystem>();
                if (targetHealthSystem != null)
                {
                    // Try to damage target found
                    bool didDamage = targetHealthSystem.TryOneShotDamage(bodyAttackParameters.damage);
    
                    if (didDamage)
                    {
                        if (bodyAttackAestheticParameters != null && bodyAttackAestheticParameters.sfxHit != null)
                        {
                            // Audio: play hit SFX
                            SfxPoolManager.Instance.PlaySfx(bodyAttackAestheticParameters.sfxHit);
                        }
                    }
                }
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                else
                {
                    Debug.LogWarningFormat(resultCollider, "[MeleeAttack] No HealthSystem component found on {0}, " +
                        "yet it contained {1} which was present on targetable layer mask {2}",
                        targetRigidbody, resultCollider, targetLayerMask);
                }
                #endif
            }
        }
    }

    /// Animation Event callback: notify script that character can cancel melee attack with another action from now on
    public void NotifyCanCancel()
    {
        m_State = State.AttackingCanCancel;
    }

    /// Animation Event callback: notify script that character has finished animation without early cancelling
    public void NotifyAnimationEnd()
    {
        m_State = State.Idle;
    }
}