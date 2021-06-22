using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Behaviour for Enemy: Ispolin
/// System for MeleeAttackIntention and ThrowIntention: handles control
/// It manages MeleeAttackIntention and ThrowIntention is therefore responsible for their Setup.
/// SEO: before MeleeAttack and Throw
[AddComponentMenu("Game/Enemy Behaviour: Ispolin")]
public class EnemyBehaviour_Ispolin : ClearableBehaviour
{
    /* Sibling components */
    
    private MeleeAttack m_MeleeAttack;
    private MeleeAttackIntention m_MeleeAttackIntention;

    private Throw m_Throw;
    private ThrowIntention m_ThrowIntention;

    
    #if UNITY_EDITOR
    
    /* Debug */

    /// Last AI behaviour chosen by this controller
    private string m_DebugLastAIBehaviourResult = "None";
    public string DebugLastAIBehaviourResult => m_DebugLastAIBehaviourResult;
    
    #endif
    
        
    private void Awake()
    {
        m_MeleeAttack = this.GetComponentOrFail<MeleeAttack>();
        m_MeleeAttackIntention = this.GetComponentOrFail<MeleeAttackIntention>();
        m_Throw = this.GetComponentOrFail<Throw>();
        m_ThrowIntention = this.GetComponentOrFail<ThrowIntention>();
    }
    
    public override void Setup()
    {
        m_MeleeAttackIntention.startAttack = false;
        
        m_ThrowIntention.startThrow = false;
        m_ThrowIntention.throwDirection = Vector2.zero;
    }
    
    private void FixedUpdate()
    {
        // Only attack if not already attacking, or attacking but can cancel
        // In practice, enemies don't really have a Cancel phase (to force them to do heavy attacks and let player
        // estimate when they'll recover), but we check both just in case.
        if ((m_MeleeAttack.State == MeleeAttackState.Idle || m_MeleeAttack.State == MeleeAttackState.AttackingCanCancel) && !m_Throw.IsThrowing)
        {
            // TODO

            // testing
            OrderThrow();
//            CheckShouldMeleeAttack();
        }
    }

    private void CheckShouldMeleeAttack()
    {
        if (InGameManager.Instance.PlayerCharacterMaster != null)
        {
            Vector2 targetPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
            if (m_MeleeAttack.IsCloseEnoughToMeleeAttack(targetPosition))
            {
                OrderMeleeAttack();

                #if UNITY_EDITOR
                m_DebugLastAIBehaviourResult = "Melee Attack";
                #endif
            }
            #if UNITY_EDITOR
            else
            {
                m_DebugLastAIBehaviourResult = "Not close enough";
            }
            #endif
        }
    }

    private void CheckShouldThrowRock()
    {
        // TODO
    }

    private void OrderMeleeAttack()
    {
        m_MeleeAttackIntention.startAttack = true;
    }

    private void OrderThrow()
    {
        // TODO
        m_ThrowIntention.startThrow = true;
        m_ThrowIntention.throwDirection = new Vector2(-1f, 1f);
    }
}
