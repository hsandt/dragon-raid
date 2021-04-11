using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// System for MoveIntention on Player character: handles control
public class PlayerMoveController : ClearableBehaviour
{
    /* Parameters data */
    
    [Tooltip("Move Parameters Data")]
    public MoveParameters moveParameters;

    
    /* Sibling components */
    
    private MoveIntention m_MoveIntention;
    
    
    private void Awake()
    {
        m_MoveIntention = this.GetComponentOrFail<MoveIntention>();
    }

    public override void Setup()
    {
        m_MoveIntention.moveVelocity = Vector2.zero;
    }

    /// PlayerInput action message callback for Move
    private void OnMove(InputValue value)
    {
        m_MoveIntention.moveVelocity = moveParameters.maxSpeed * value.Get<Vector2>();
    }
}
