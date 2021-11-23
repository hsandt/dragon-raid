using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for Rigidbody2D and MoveGroundedIntention: handles move on ground
public class MoveGrounded : ClearableBehaviour
{
    /* Animator hashes */

    private static readonly int jumpHash = Animator.StringToHash("Jump");
    
    
    /* Sibling components */
    
    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody2D;
    private MoveGroundedIntention m_MoveGroundedIntention;
    
    
    /* State */

    /// Velocity stored before Pause, used to restore state on Resume
    private Vector2 m_VelocityOnResume;

    
    private void Awake()
    {
        m_Animator = this.GetComponentOrFail<Animator>();
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
        m_MoveGroundedIntention = this.GetComponentOrFail<MoveGroundedIntention>();
    }

    public override void Setup()
    {
        m_Rigidbody2D.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        // Grounded characters naturally move with the environment, which is now static (the camera moves instead),
        // so no extra velocity added due to scrolling
        Vector2 newVelocity = m_Rigidbody2D.velocity;

        // Set velocity Y when ordered to jump (we don't check if we are grounded, as we assume
        // character starts grounded and only tries to jump once)
        
        if (m_MoveGroundedIntention.jumpSpeedImpulse > 0f)
        {
            // Set velocity Y by consuming jump speed impulse intention
            newVelocity.y = m_MoveGroundedIntention.jumpSpeedImpulse;
            m_MoveGroundedIntention.jumpSpeedImpulse = 0f;
            
            // Animation: play Jump animation
            m_Animator.SetTrigger(jumpHash);
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
