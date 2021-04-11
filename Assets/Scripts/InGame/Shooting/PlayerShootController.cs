using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// System for ShootIntention on Player character: handles control
public class PlayerShootController : ClearableBehaviour
{
    /* Sibling components */
    
    private ShootIntention m_ShootIntention;
    
    
    private void Awake()
    {
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();
    }

    public override void Setup()
    {
        m_ShootIntention.holdFire = false;
    }

    /// PlayerInput action message callback for Shoot
    private void OnFire(InputValue value)
    {
        m_ShootIntention.holdFire = value.isPressed;
    }
}
