using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Move Grounded Intention data component
public class MoveGroundedIntention : MonoBehaviour
{
    [ReadOnlyField, Tooltip("Intended move ground speed (positive right)")]
    public float groundSpeed;
    
    [ReadOnlyField, Tooltip("Positive when character wants to jump, set to target jump speed (consumed)")]
    public float jumpSpeedImpulse;
}
