using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// System for MoveIntention on Player character: handles control
public class EnemyMoveController : BaseMoveController
{
    private void FixedUpdate()
    {
        // for now, simple logic: enemy moves as much as they can
        m_MoveIntention.moveVelocity = moveParameters.maxSpeed * Vector2.left;
    }
}
