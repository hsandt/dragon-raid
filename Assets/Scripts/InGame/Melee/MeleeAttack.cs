using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityConstants;
using UnityEngine;

/// System that deals damage to an opponent's Health on melee attack hit
/// Collision layers: EnemyMeleeHitBox collides with PlayerHurtBox, PlayerMeleeHitBox colliders with EnemyHurtBox 
public class MeleeAttack : MonoBehaviour
{
    /// Colliders used internally to store physics 2D query results without allocations
    private static readonly Collider2D[] resultColliders;

    
    /* Child component references */

    [Tooltip("RectContainer describing the Melee Hit Box")]
    public RectContainer meleeHitBox;
    

    /* Parameters data */
    
    [Tooltip("Melee Attack Parameters Data")]
    public MeleeAttackParameters bodyAttackParameters;
    
    [Tooltip("Melee Attack Visual Parameters Data")]
    public MeleeAttackAestheticParameters bodyAttackAestheticParameters;
    
    
    /* Parameters */
    
    [SerializeField, Tooltip("Faction the attacker belongs to")]
    private Faction m_AttackerFaction;

    
    private void Awake()
    {
        Debug.AssertFormat(bodyAttackParameters, this,
            "[MeleeAttack] Melee Attack Parameters not set on Melee Attack component {0}", this);
    }

    private int GetOpponentHurtBoxLayerMask(Faction faction)
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

    /// Start melee attack with animation and hitbox
    public void StartAttack()
    {
        // Animation: play Melee Attack animation
        // TODO
        
        if (bodyAttackAestheticParameters != null && bodyAttackAestheticParameters.sfxAttack != null)
        {
            // Audio: play attack SFX
            SfxPoolManager.Instance.PlaySfx(bodyAttackAestheticParameters.sfxAttack);
        }
        
        // for now, no lag, attack effect is instant
        TryDamageOpponentsInHitBox();
    }

    private void TryDamageOpponentsInHitBox()
    {
        int targetLayerMask = GetOpponentHurtBoxLayerMask(m_AttackerFaction);
        int resultsCount = Physics2D.OverlapAreaNonAlloc(meleeHitBox.worldMin, meleeHitBox.worldMax, resultColliders, targetLayerMask);

        for (int i = 0; i < resultsCount; ++i)
        {
            var resultCollider = resultColliders[i];
            var targetHealthSystem = resultCollider.GetComponent<HealthSystem>();
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
                                                       "yet it was present on targetable layer {1}", resultCollider,
                    targetLayerMask);
            }
            #endif
        }
    }
}
