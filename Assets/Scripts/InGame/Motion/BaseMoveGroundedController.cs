using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// Base system class for MoveGroundedIntention on Player or Enemy character: handles control
/// SEO for concrete child classes: before MoveGrounded
public abstract class BaseMoveGroundedController : ClearableBehaviour
{
    [Header("Parameters data")]

    [Tooltip("Move Grounded Parameters Data (must match the one on Move Grounded component)")]
    public MoveGroundedParameters moveGroundedParameters;
    
    
    /* Sibling components */
    
    protected MoveGroundedIntention m_MoveGroundedIntention;
    
    
    private void Awake()
    {
        m_MoveGroundedIntention = this.GetComponentOrFail<MoveGroundedIntention>();

        Init();
    }

    /// Override this method to customize Awake behavior while preserving base logic
    protected virtual void Init() {}

    public override void Setup()
    {
        m_MoveGroundedIntention.groundSpeed = 0f;
        m_MoveGroundedIntention.jump = false;
    }
}
