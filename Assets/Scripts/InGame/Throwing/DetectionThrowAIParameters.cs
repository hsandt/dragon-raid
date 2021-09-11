using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Throw parameters for AI Detection-based Throw
[CreateAssetMenu(fileName = "DetectionThrowAIParameters", menuName = "Data/Detection Throw AI Parameters")]
public class DetectionThrowAIParameters : ScriptableObject
{
    [Tooltip("Maximum upward angle where Ispolin can detect a target, from Throw Detection Origin (he can detect anything below)")]
    public float maxDetectionUpwardAngle = 45f;
    
    [Tooltip("Minimum forward distance to target on X required to detect it")]
    public float minDetectionDistanceX = 2f;
    
    [Tooltip("Maximum distance where Ispolin can detect a target")]
    public float maxDetectionDistance = 8f;
}
