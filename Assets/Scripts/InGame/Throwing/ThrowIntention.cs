using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Throw intention data component
public class ThrowIntention : ClearableBehaviour
{
    [ReadOnlyField, Tooltip("Does the character want to throw a projectile?")]
    public bool startThrow;

    [ReadOnlyField, Tooltip("Throw direction. Can be always the same, or target a specific object. " +
                            "No need to normalize, it will be normalized on throw.")]
    public Vector2 throwDirection;

    [ReadOnlyField, Tooltip("Initial projectile speed (m/s)")]
    public float throwSpeed;

    public override void Clear()
    {
        startThrow = false;
        throwDirection = Vector2.zero;
        throwSpeed = 0f;
    }
}
