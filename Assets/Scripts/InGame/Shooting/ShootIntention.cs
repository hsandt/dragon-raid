using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Shoot intention data component
public class ShootIntention : MonoBehaviour
{
    [ReadOnlyField, Tooltip("Does the character want to keep firing? (cooldown still applies, causing periodical fire)")] 
    public bool holdFire;
    
    [ReadOnlyField, Tooltip("Shooting direction. Can be always the same, or target a specific object. " +
                            "No need to normalize, it will be normalized on shoot.")] 
    public Vector2 fireDirection;
}
