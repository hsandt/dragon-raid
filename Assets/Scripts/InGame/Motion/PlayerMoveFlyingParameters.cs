using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Base move parameters for flying characters
[CreateAssetMenu(fileName = "PlayerMoveFlyingParameters", menuName = "Data/Player Move Flying Parameters")]
public class PlayerMoveFlyingParameters : ScriptableObject
{
    [Tooltip("Maximum speed (m/s)")]
    public float maxSpeed = 4f;
}
