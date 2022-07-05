using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Move parameters for flying player character
[CreateAssetMenu(fileName = "PlayerMoveFlyingParameters", menuName = "Data/Player Move Flying Parameters")]
public class PlayerMoveFlyingParameters : ScriptableObject
{
    [Tooltip("Maximum speed (m/s)")]
    public float maxSpeed = 4f;
}
