using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// System for MoveIntention on grounded Enemy: handles control
public class EnemyMoveController_Grounded : BaseMoveController
{
    [Header("Parameters data")]

    [Tooltip("Move Parameters Data")]
    public MoveParameters_Grounded moveParametersGrounded;
    
    
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;


    protected override void Init()
    {
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // no slops for now, so moving on ground just means linear motion to the left
        m_MoveIntention.moveVelocity = moveParametersGrounded.maxGroundSpeed * Vector2.left;
    }
}
