using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Move parameters for grounded enemies
[CreateAssetMenu(fileName = "MoveGroundedParameters", menuName = "Data/Move Grounded Parameters")]
public class MoveGroundedParameters : ScriptableObject
{
    [Tooltip("Maximum ground speed (m/s)")]
    public float maxGroundSpeed = 4f;
    
    [Tooltip("Maximum jump speed on Y axis (m/s). Also used to deduce max jump height.")]
    public float maxJumpSpeed = 12f;
    
    [Tooltip("Distance under which player character is considered near along X axis")]
    public float playerCharacterDetectionDistanceX = 5f;
}
