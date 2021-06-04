using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Base system class for MeleeAttackIntention on Player or Enemy character: handles control
/// SEO for concrete child classes: before MeleeAttack
public abstract class BaseMeleeAttackController : ClearableBehaviour
{
    /* Sibling components */
    
    protected MeleeAttackIntention m_MeleeAttackIntention;
    
    
    private void Awake()
    {
        m_MeleeAttackIntention = this.GetComponentOrFail<MeleeAttackIntention>();
    }
    
    public override void Setup()
    {
        m_MeleeAttackIntention.startAttack = false;
    }
}
