using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;

/// Controller for Shoot on Player character
public class PlayerShootController : MonoBehaviour
{
    /* Parameters data */
    
    public ShootParameters shootParameters;

    
    /* Sibling components */
    
    private ShootIntention m_ShootIntention;
    
    
    private void Awake()
    {
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();
    }
    
    /// PlayerInput action message callback for Shoot
    private void OnFire(InputValue value)
    {
        m_ShootIntention.fire = value.isPressed;
    }
}
