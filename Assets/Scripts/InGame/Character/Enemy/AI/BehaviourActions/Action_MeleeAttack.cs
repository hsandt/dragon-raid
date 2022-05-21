using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to order a Melee Attack
[AddComponentMenu("Game/Action: Melee Attack")]
public class Action_MeleeAttack : BehaviourAction
{
    /* Owner sibling components */

    private MeleeAttack m_MeleeAttack;
    private MeleeAttackIntention m_MeleeAttackIntention;


    /* State */

    /// True when action has ordered throw
    private bool hasOrderedMeleeAttack;


    protected override void OnInit()
    {
        m_MeleeAttack = m_EnemyCharacterMaster.GetComponentOrFail<MeleeAttack>();
        m_MeleeAttackIntention = m_MeleeAttack.MeleeAttackIntention;
    }

    public override void OnStart()
    {
        hasOrderedMeleeAttack = false;
    }

    public override void RunUpdate()
    {
        m_MeleeAttackIntention.startAttack = true;
        hasOrderedMeleeAttack = true;
    }

    protected override bool IsOver()
    {
        return hasOrderedMeleeAttack;
    }

    #if UNITY_EDITOR
    public override string GetNodeName()
    {
        return "Melee Attack";
    }
    #endif
}
