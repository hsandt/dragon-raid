using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// Base system class for MoveFlyingIntention on Player or Enemy character: handles control
/// It manages MoveFlyingIntention and is therefore responsible for its Setup.
/// SEO for concrete child classes: before MoveFlying
public abstract class BaseMoveFlyingController : ClearableBehaviour
{
    [Header("Parameters data")]

    [Tooltip("Move Parameters Data")]
    public MoveFlyingParameters MoveFlyingParameters;

    
    /* Sibling components */
    
    protected MoveFlyingIntention m_MoveFlyingIntention;
    
    
    private void Awake()
    {
        m_MoveFlyingIntention = this.GetComponentOrFail<MoveFlyingIntention>();

        Init();
    }

    /// Override this method to customize Awake behavior while preserving base logic
    protected virtual void Init() {}

    public override void Setup()
    {
        m_MoveFlyingIntention.moveVelocity = Vector2.zero;
    }
}
