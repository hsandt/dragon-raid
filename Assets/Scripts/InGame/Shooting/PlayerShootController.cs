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
        m_ShootIntention.holdFire = value.isPressed;
        m_ShootIntention.fireDirection = m_Shoot.shootAnchor.right;
    }
}
