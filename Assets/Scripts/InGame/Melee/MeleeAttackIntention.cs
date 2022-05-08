using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Melee Attack intention data component
public class MeleeAttackIntention : ClearableBehaviour
{
    [ReadOnlyField, Tooltip("Does the character want to start melee attack?")]
    public bool startAttack;

    public override void Clear()
    {
        startAttack = false;
    }
}
