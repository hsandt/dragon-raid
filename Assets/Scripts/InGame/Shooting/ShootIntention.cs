using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Shoot intention data component
public class ShootIntention : MonoBehaviour
{
    [ReadOnlyField, Tooltip("Does the character want to keep firing? Cooldown still applies, causing periodical fire. " +
        "Has priority over Fire Once (still get consumed if both are set).")] 
    public bool holdFire;
    
    [ReadOnlyField, Tooltip("Does the character want to fire a single bullet? Unlike Hold Fire, gets consumed on usage.")] 
    public bool fireOnce; 
    
    [ReadOnlyField, Tooltip("Shooting direction. Can be always the same, or target a specific object. " +
        "No need to normalize, it will be normalized on shoot.")] 
    public Vector2 fireDirection;
}
