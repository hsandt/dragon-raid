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
    
    
    private void Awake()
    {
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();
    }
    
    public override void Setup()
    {
        m_ShootIntention.holdFire = false;
    }
}
