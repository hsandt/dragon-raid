using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// System for MoveFlyingIntention on Enemy: handles control
public class EnemyMoveFlyingController : BaseMoveFlyingController
{
    [Header("Individual parameters")]
    
    [SerializeField, Tooltip("Fixed direction in which the enemy moves (all enemies of the same type will move the same)")]
    private Vector2 moveDirection = Vector2.left;

#if UNITY_EDITOR
    public Vector2 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }
#endif
    
    
    private void FixedUpdate()
    {
        // linear motion
        m_MoveFlyingIntention.moveVelocity = MoveFlyingParameters.maxSpeed * moveDirection.normalized;
    }
}
