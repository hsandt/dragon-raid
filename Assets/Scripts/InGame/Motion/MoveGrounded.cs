using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for Rigidbody2D and MoveGroundedIntention: handles move on ground
public class MoveGrounded : ClearableBehaviour
{
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    private MoveGroundedIntention m_MoveGroundedIntention;
    
    
    /* State */

    /// Velocity stored before Pause, used to restore state on Resume
    private Vector2 m_VelocityOnResume;

    
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
        // Note we have set the motion intention relative to ground, so we always add the reverse ground scrolling
        // speed, even in the air. This way, a grounded unit with an extra ground speed of 0 sticks to scrolling speed. 
        newVelocity.x = ScrollingManager.Instance.ComputeTotalSpeedWithScrolling(m_MoveGroundedIntention.signedGroundSpeed);

        // Set velocity Y when ordered to jump (we don't check if we are grounded, as we assume
        // character starts grounded and only tries to jump once)
        
        if (m_MoveGroundedIntention.jumpSpeedImpulse > 0f)
        {
            // Set velocity Y by consuming jump speed impulse intention
            newVelocity.y = m_MoveGroundedIntention.jumpSpeedImpulse;
            m_MoveGroundedIntention.jumpSpeedImpulse = 0f;
        }
        
        m_Rigidbody2D.velocity = newVelocity;
    }
    
    /// Pause behaviour
    private void OnDisable()
    {
        m_VelocityOnResume = m_Rigidbody2D.velocity;
        m_Rigidbody2D.velocity = Vector2.zero;
    }
    
    /// Resume behaviour
    private void OnEnable()
    {
        // Restoring velocity is not critical since it is set every Fixed Update, but safer
        m_Rigidbody2D.velocity = m_VelocityOnResume;
    }
}
