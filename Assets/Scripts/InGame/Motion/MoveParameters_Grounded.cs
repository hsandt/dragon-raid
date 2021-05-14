using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Move parameters for grounded enemies
[CreateAssetMenu(fileName = "MoveParameters_Grounded", menuName = "Data/Move Parameters - Grounded")]
public class MoveParameters_Grounded : ScriptableObject
{
    [Tooltip("Maximum ground speed (m/s)")]
    public float maxGroundSpeed = 4f;
    
    [Tooltip("Maximum jump speed on Y axis (m/s). Also used to deduce max jump height.")]
    public float maxJumpSpeed = 12f;
}
