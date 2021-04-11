using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for ShootIntention on Enemy character: handles control
public class EnemyShootController : ClearableBehaviour
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

    private void FixedUpdate()
    {
        // for now, simple logic: enemy shoots as much as they can
        m_ShootIntention.holdFire = true;
    }
}
