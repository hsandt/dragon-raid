using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;

/// System for ShootIntention on Player character: handles control
public class PlayerShootController : MonoBehaviour
{
    /* Parameters data */
    
    [Tooltip("Shoot Parameters Data. Must match the one on Shoot component.")]
    public ShootParameters shootParameters;

    
    /* Sibling components */
    
    private ShootIntention m_ShootIntention;
    
    
    /* Cache */

    /// Cache whether fire input is pressed or not (avoids direct input state access)
    private bool m_IsFireInputPressed;
    
    
    /* State */
    
    /// Time before next Fire is allowed
    private float m_FireCooldownTime;
    
    
    private void Awake()
    {
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();

        m_IsFireInputPressed = false;
    }

    private void Start()
    {
        m_FireCooldownTime = 0f;
    }

    private void FixedUpdate()
    {
        if (m_FireCooldownTime > 0f)
        {
            m_FireCooldownTime = Mathf.Max(0f, m_FireCooldownTime - Time.deltaTime);
        }
        
        // do not put this in else case of countdown below, in case we've just ended cooldown this frame
        if (m_IsFireInputPressed && m_FireCooldownTime <= 0f)
        {
            // set intention to fire
            m_ShootIntention.fire = true;
        
            // set cooldown time to prevent firing immediately again
            m_FireCooldownTime = shootParameters.fireCooldownDuration;
        }
    }

    /// PlayerInput action message callback for Shoot
    private void OnFire(InputValue value)
    {
        m_IsFireInputPressed = value.isPressed;
    }
}
