using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// System for ShootIntention on Player character: handles control
/// SEO: before Shoot
public class PlayerShootController : BaseShootController
{
    /// PlayerInput action message callback for Shoot
    private void OnFire(InputValue value)
    {
        // When holding fire, do not Add fire directions to the intention.
        // Instead, Shoot will compute fire direction live for each shot.     
        m_ShootIntention.holdFire = value.isPressed;
    }
}
