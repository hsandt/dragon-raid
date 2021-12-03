using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for Rigidbody2D and MoveFlyingIntention: handles move
public class MoveFlying : ClearableBehaviour
{
    [Header("Parameters data")]

    [Tooltip("Move Flying Parameters Data")]
    public MoveFlyingParameters moveFlyingParameters;

    
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    private MoveFlyingIntention m_MoveFlyingIntention;
    
    
    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(moveFlyingParameters != null, this, "[MoveFlying] Awake: Move Flying Parameters not set on {0}", this);
        #endif
        
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

        if (moveFlyingParameters.moveRelativelyToScreen)
        {
            m_Rigidbody2D.velocity += ScrollingManager.Instance.ScrollingSpeed * Vector2.right;
        }
    }
}
