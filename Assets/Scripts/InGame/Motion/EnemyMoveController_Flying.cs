using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// System for MoveIntention on Enemy: handles control
public class EnemyMoveController_Flying : BaseMoveController
{
    [Header("Parameters data")]

    [Tooltip("Move Parameters Data")]
    public MoveParameters_Flying moveParametersFlying;


    [Header("Individual parameters")]
    
    [SerializeField, Tooltip("Fixed direction in which the enemy moves (all enemies of the same type will move the same)")]
    private Vector2 moveDirection = Vector2.left;

#if UNITY_EDITOR
    public Vector2 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }
#endif
    
    
    private void FixedUpdate()
    {
        // linear motion
        m_MoveIntention.moveVelocity = moveParametersFlying.maxSpeed * moveDirection.normalized;
    }
}
