using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Move Intention data component
public class MoveIntention : MonoBehaviour
{
    [ReadOnlyField, Tooltip("Intended move velocity")]
    public Vector2 moveVelocity;
}
