using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for Rigidbody2D and MoveGroundedIntention: handles move on ground
public class MoveGrounded : ClearableBehaviour
{
    [Header("Parameters data")]

    [Tooltip("Move Grounded Parameters Data")]
    public MoveGroundedParameters moveGroundedParameters;

    
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    private MoveGroundedIntention m_MoveGroundedIntention;
    
    private void Awake()
    {
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
        m_MoveGroundedIntention = this.GetComponentOrFail<MoveGroundedIntention>();
    }

    public override void Setup()
    {
        m_Rigidbody2D.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Vector2 newVelocity = m_Rigidbody2D.velocity;
        
        // When grounded: no slopes for now, so just move left/right according to ground speed
        // (gravity is still applied but ground collider will prevent sinking into ground)
        // When airborne: only set X component of velocity to let gravity do its job
        // Setting X component of velocity covers both cases
        newVelocity.x = m_MoveGroundedIntention.groundSpeed;
        
        // Set velocity Y when ordered to jump (we don't check if we are grounded, as we assume
        // character starts grounded and only tries to jump once)
        
        if (ControlUtil.ConsumeBool(ref m_MoveGroundedIntention.jump))
        {
            newVelocity.y = moveGroundedParameters.maxJumpSpeed;
        }
        
        m_Rigidbody2D.velocity = newVelocity;
    }
}
