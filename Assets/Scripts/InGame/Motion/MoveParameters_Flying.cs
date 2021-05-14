using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Move parameters for flying enemies
[CreateAssetMenu(fileName = "MoveParameters_Flying", menuName = "Data/Move Parameters - Flying")]
public class MoveParameters_Flying : ScriptableObject
{
    [Tooltip("Maximum speed (m/s)")]
    public float maxSpeed = 4f;
}
