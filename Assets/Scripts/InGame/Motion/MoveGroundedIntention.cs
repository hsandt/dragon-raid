using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Move Grounded Intention data component
public class MoveGroundedIntention : MonoBehaviour
{
    [ReadOnlyField, Tooltip("Intended move ground speed (positive right)")]
    public float groundSpeed;
    
    [ReadOnlyField, Tooltip("True when character wants to jump (consumed)")]
    public bool jump;
}
