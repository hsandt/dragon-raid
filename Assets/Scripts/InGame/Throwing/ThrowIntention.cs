using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Throw intention data component
public class ThrowIntention : MonoBehaviour
{
    [ReadOnlyField, Tooltip("Does the character want to throw a projectile?")] 
    public bool startThrow;
    
    [ReadOnlyField, Tooltip("Throw direction. Can be always the same, or target a specific object. " +
                            "No need to normalize, it will be normalized on throw.")] 
    public Vector2 throwDirection;
}
