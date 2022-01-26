using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonsHelper;

/// Action to start moving grounded character at given speed
/// This is a fire-and-forget action, which allows to do another actions
/// after a sticky intention has been set. Stop it with another
/// Action: Move Grounded Toward at speed 0.
[AddComponentMenu("Game/Action: Start Move Grounded Toward")]
public class Action_StartMoveGroundedToward : BehaviourAction
{
    [Header("Parameters")] [SerializeField, Tooltip("Motion signed speed (m/s) (positive to go right)")]
    private float signedSpeed = 1f;

    #if UNITY_EDITOR
    public float SignedSpeed
    {
        get => signedSpeed;
        set => signedSpeed = value;
    }
    #endif


    /* Owner sibling components */

    private MoveGroundedIntention m_MoveGroundedIntention;


    protected override void OnInit()
    {
        m_MoveGroundedIntention = m_EnemyCharacterMaster.GetComponentOrFail<MoveGroundedIntention>();
    }

    public override void OnStart()
    {
        m_MoveGroundedIntention.signedGroundSpeed = signedSpeed;
    }

    protected override bool IsOver()
    {
        // Return true to immediately end the action
        // We set the signed ground speed in OnStart, not RunUpdate, so we don't need
        // to run this action at all
        return true;
    }
}