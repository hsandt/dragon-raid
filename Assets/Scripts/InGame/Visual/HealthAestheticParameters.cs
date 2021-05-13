using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Health Aesthetic parameters for Health System
[CreateAssetMenu(fileName = "HealthAestheticParameters", menuName = "Data/Health Aesthetic Parameters")]
public class HealthAestheticParameters : ScriptableObject
{
    [Tooltip("Brightness value when entity is damaged. 0f to keep original color, 1f for full white.")]
    [Range(0f, 1f)]
    public float damagedBrightness = 1f;
    
    [Tooltip("Duration of brightness effect when entity is damaged (s)")]
    [Range(0f, 0.2f)]
    public float damagedBrightnessDuration = 0.1f;
}
