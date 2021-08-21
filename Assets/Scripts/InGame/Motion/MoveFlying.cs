using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for Rigidbody2D and MoveFlyingIntention: handles move
public class MoveFlying : ClearableBehaviour
{
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    private MoveFlyingIntention m_MoveFlyingIntention;
    
    
    /* State */

    /// Velocity stored before Pause, used to restore state on Resume
    private Vector2 m_VelocityOnResume;
    
    
    private void Awake()
    {
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
        m_MoveFlyingIntention = this.GetComponentOrFail<MoveFlyingIntention>();
    }

    public override void Setup()
    {
        m_Rigidbody2D.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        m_Rigidbody2D.velocity = m_MoveFlyingIntention.moveVelocity;
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
