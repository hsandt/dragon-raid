using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Base system class for ShootIntention on Player or Enemy character: handles control
public abstract class BaseShootController : ClearableBehaviour
{
    /* Sibling components */
    
    protected ShootIntention m_ShootIntention;
    protected Shoot m_Shoot;
    
    
    private void Awake()
    {
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();
        m_Shoot = this.GetComponentOrFail<Shoot>();
    }
    
    public override void Setup()
    {
        m_ShootIntention.holdFire = false;
        m_ShootIntention.fireDirection = Vector2.zero;
    }
}
