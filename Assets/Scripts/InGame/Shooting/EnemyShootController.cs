using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// System for ShootIntention on Enemy character: handles control
public class EnemyShootController : BaseShootController
{
    private void FixedUpdate()
    {
        // for now, simple logic: enemy shoots as much as they can
        m_ShootIntention.holdFire = true;
    }
}
