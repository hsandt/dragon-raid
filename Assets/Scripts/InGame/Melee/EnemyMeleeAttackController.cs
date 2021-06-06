using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;

/// System for MeleeAttackIntention on Enemy character: handles control
/// SEO: before MeleeAttack
public class EnemyMeleeAttackController : BaseMeleeAttackController
{
    /* Sibling components */
    
    private MeleeAttack m_MeleeAttack;
    
    
    #if UNITY_EDITOR
    
    /* Debug */

    /// Last AI behaviour chosen by this controller
    private string m_DebugLastAIBehaviourResult = "None";
    public string DebugLastAIBehaviourResult => m_DebugLastAIBehaviourResult;
    
    #endif
    
    
    protected override void Init()
    {
        base.Init();
        
        m_MeleeAttack = this.GetComponentOrFail<MeleeAttack>();
    }
    
    private void FixedUpdate()
    {
        // Only attack if not already attacking, or attacking but can cancel
        // In practice, enemies don't really have a Cancel phase (to force them to do heavy attacks and let player
        // estimate when they'll recover), but we check both just in case.
        if (m_MeleeAttack.State == MeleeAttackState.Idle || m_MeleeAttack.State == MeleeAttackState.AttackingCanCancel)
        {
            CheckShouldAttack();
        }
    }

    private void CheckShouldAttack()
    {
        if (InGameManager.Instance.PlayerCharacterMaster != null)
        {
            Vector2 targetPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
            if (IsCloseEnoughToMeleeAttack(targetPosition))
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
    
    /// Return true is enemy is facing target, close enough to hit it with a Melee Attack
    private bool IsCloseEnoughToMeleeAttack(Vector2 targetPosition)
    {
        // Be pragmatic: check if the player character center is inside the melee hit box world rect
        // If this makes the enemy AI too "optimal" and therefore too strong in terms of reactivity, add a bit of margin
        // (shrink test rectangle compared to hit box)
        return m_MeleeAttack.meleeHitBox.worldRect.Contains(targetPosition);
    }

    private void OrderMeleeAttack()
    {
        m_MeleeAttackIntention.startAttack = true;
    }
}
