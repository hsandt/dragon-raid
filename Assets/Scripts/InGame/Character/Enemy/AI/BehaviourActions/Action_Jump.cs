using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to jump
[AddComponentMenu("Game/Action: Jump")]
public class Action_Jump : BehaviourAction
{
    [Header("Parameters")]

    [SerializeField, Tooltip("Jump speed impulse (m/s)")]
    [Min(0f)]
    private float jumpSpeedImpulse = 0f;

    #if UNITY_EDITOR
    public float JumpSpeedImpulse { get => jumpSpeedImpulse; set => jumpSpeedImpulse = value; }
    #endif


    /* Owner sibling components */

    private MoveGroundedIntention m_MoveGroundedIntention;


    /* State */

    /// True when character has ordered jump
    private bool m_HasOrderedJump;


    protected override void OnInit()
    {
        m_MoveGroundedIntention = m_EnemyCharacterMaster.GetComponentOrFail<MoveGroundedIntention>();
    }

    public override void OnStart()
    {
        m_HasOrderedJump = false;
    }

    public override void RunUpdate()
    {
        // REFACTOR: we could also set this OnStart so we can let IsOver return true like Action_StartMoveGroundedToward
        // in a fire-and-forget way, without needing any flag; although that means that OnStart will have a side effect.
        m_MoveGroundedIntention.jumpSpeedImpulse = jumpSpeedImpulse;
        m_HasOrderedJump = true;
    }

    protected override bool IsOver()
    {
        return m_HasOrderedJump;
    }
}
