using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// System for MoveIntention on Player character: handles control
public class PlayerMoveController : BaseMoveController
{
    /// PlayerInput action message callback for Move
    private void OnMove(InputValue value)
    {
        // Binary Cardinal Processor makes sure that move input is -1/0/+1 on each axis
        m_MoveIntention.moveVelocity = moveParameters.maxSpeed * value.Get<Vector2>();
    }
}
