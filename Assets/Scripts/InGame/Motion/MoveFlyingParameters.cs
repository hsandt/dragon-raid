using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Move parameters for flying enemies
[CreateAssetMenu(fileName = "MoveFlyingParameters", menuName = "Data/Move Flying Parameters")]
public class MoveFlyingParameters : ScriptableObject
{
    [Tooltip("Maximum speed (m/s)")]
    public float maxSpeed = 4f;
}
