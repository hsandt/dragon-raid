using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Base system class for MeleeAttackIntention on Player or Enemy character: handles control
/// It manages MeleeAttackIntention and is therefore responsible for its Setup.
/// SEO for concrete child classes: before MeleeAttack
public abstract class BaseMeleeAttackController : MonoBehaviour
{
    /* Sibling components */

    protected MeleeAttackIntention m_MeleeAttackIntention;


    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        m_MeleeAttackIntention = this.GetComponentOrFail<MeleeAttackIntention>();
    }
}
