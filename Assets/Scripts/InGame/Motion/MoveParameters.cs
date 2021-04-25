using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Move parameters
[CreateAssetMenu(fileName = "MoveParameters", menuName = "Data/Move Parameters")]
public class MoveParameters : ScriptableObject
{
    [Tooltip("Maximum speed (m/s)")]
    public float maxSpeed = 4f;
}
