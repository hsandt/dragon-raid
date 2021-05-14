using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// Base system class for MoveIntention on Player or Enemy character: handles control
public abstract class BaseMoveController : ClearableBehaviour
{
    /* Sibling components */
    
    protected MoveIntention m_MoveIntention;
    
    
    private void Awake()
    {
        m_MoveIntention = this.GetComponentOrFail<MoveIntention>();

        Init();
    }

    /// Override this method to customize Awake behavior while preserving base logic
    protected virtual void Init() {}

    public override void Setup()
    {
        m_MoveIntention.moveVelocity = Vector2.zero;
    }
}
