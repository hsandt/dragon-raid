using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// System for ShootIntention on Enemy character: handles control
/// SEO: before Shoot
public class EnemyShootController : BaseShootController
{
    private void FixedUpdate()
    {
        // When holding fire, do not Add fire directions to the intention.
        // Instead, Shoot will compute fire direction live for each shot.
        m_ShootIntention.holdFire = true;
    }
}
