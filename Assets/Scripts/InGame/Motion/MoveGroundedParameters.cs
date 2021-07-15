using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Move parameters for grounded enemies
[CreateAssetMenu(fileName = "MoveGroundedParameters", menuName = "Data/Move Grounded Parameters")]
public class MoveGroundedParameters : ScriptableObject
{
    [Tooltip("Maximum ground speed (m/s). Set to 0 to stick to ground (follow scrolling speed).")]
    [Range(0f, 4f)]
    public float maxGroundSpeed = 2f;
    
    [Tooltip("Maximum jump speed on Y axis (m/s). Also used to deduce max jump height. Set to 0 to disable jumps.")]
    [Range(0f, 30f)]
    public float maxJumpSpeed = 15f;
}
