using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Move Intention data component
public class MoveFlyingIntention : ClearableBehaviour
{
    [ReadOnlyField, Tooltip("Intended move velocity")]
    public Vector2 moveVelocity;

    public override void Clear()
    {
        moveVelocity = Vector2.zero;
    }
}
