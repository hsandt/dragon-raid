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
    private MoveFlyingIntention m_MoveIntention;
    
    private void Awake()
    {
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
        m_MoveIntention = this.GetComponentOrFail<MoveFlyingIntention>();
    }

    public override void Setup()
    {
        m_Rigidbody2D.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        m_Rigidbody2D.velocity = m_MoveIntention.moveVelocity;
    }
}
