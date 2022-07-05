using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Base system class for ShootIntention on Player or Enemy character: handles control
/// It manages ShootIntention and is therefore responsible for its Setup.
/// SEO for concrete child classes: before Shoot
public abstract class BaseShootController : MonoBehaviour
{
    /* Sibling components */

    protected ShootIntention m_ShootIntention;


    private void Awake()
    {
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();
    }
}
