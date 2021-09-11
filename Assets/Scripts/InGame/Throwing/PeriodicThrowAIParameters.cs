using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Throw parameters for AI Periodic Throw
[CreateAssetMenu(fileName = "PeriodicThrowAIParameters", menuName = "Data/Periodic Throw AI Parameters")]
public class PeriodicThrowAIParameters : ScriptableObject
{
    [Tooltip("Initial delay before first throw (useful to avoid enemy just spawned and already throwing from screen edge) (s)")]
    public float initialDelay = 1f;
    
    [Tooltip("Period between two throws (s)")]
    public float period = 1f;
    
    [Tooltip("Throw angle (degrees, CW from left)")]
    public float angle = 45f;
}
