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
        m_MoveIntention.moveVelocity = moveParameters.maxSpeed * value.Get<Vector2>();
    }
}
