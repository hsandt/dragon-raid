using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Move Grounded Intention data component
public class MoveGroundedIntention : ClearableBehaviour
{
    [ReadOnlyField, Tooltip("Intended extra move ground speed (positive right). " +
                            "Always added to MINUS scrolling speed (even in the air).")]
    public float signedGroundSpeed;

    [ReadOnlyField, Tooltip("Positive when character wants to jump, set to target jump speed (consumed)")]
    public float jumpSpeedImpulse;

    public override void Clear()
    {
        signedGroundSpeed = 0f;
        jumpSpeedImpulse = 0f;
    }
}
