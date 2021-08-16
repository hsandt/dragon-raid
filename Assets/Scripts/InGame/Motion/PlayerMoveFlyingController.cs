using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// System for MoveFlyingIntention on Player character: handles control
/// SEO: before MoveFlying
public class PlayerMoveFlyingController : BaseMoveFlyingController
{
    [Header("Parameters data")]

    [Tooltip("Move Parameters Data")]
    public PlayerMoveFlyingParameters playerMoveFlyingParameters;
    
    
    /// PlayerInput action message callback for Move
    private void OnMove(InputValue value)
    {
        // Binary Cardinal Processor makes sure that move input is -1/0/+1 on each axis
        m_MoveFlyingIntention.moveVelocity = playerMoveFlyingParameters.maxSpeed * value.Get<Vector2>();
    }
}
