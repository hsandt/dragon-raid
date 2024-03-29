using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Shoot intention data component
public class ShootIntention : ClearableBehaviour
{
    [ReadOnlyField, Tooltip("Does the character want to keep firing? Cooldown still applies, causing periodical fire. " +
        "Has priority over Fire Once (still get consumed if both are set).")]
    public bool holdFire;

    [ReadOnlyField, Tooltip("Does the character want to fire a single bullet? Unlike Hold Fire, gets consumed on usage.")]
    public bool fireOnce;

    [ReadOnlyField, Tooltip("List of bullet velocities for each bullet to shoot this frame. Consumed every frame.")]
    public List<Vector2> bulletVelocities = new();

    public override void Clear()
    {
        holdFire = false;
        fireOnce = false;
        bulletVelocities.Clear();
    }
}
