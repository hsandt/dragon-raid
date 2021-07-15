using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Shared health parameters for Health System, both gameplay and aesthetics
/// There are no variations between entities, so there should be only one instance of this.
[CreateAssetMenu(fileName = "HealthSharedParameters", menuName = "Data/Health Shared Parameters")]
public class HealthSharedParameters : ScriptableObject
{
    [Header("Gameplay")]

    [Tooltip("Duration of invincibility after receiving a body attack (to avoid attack stacking)")]
    [Range(0f, 2f)]
    public float postBodyAttackInvincibilityDuration = 1f;
    
    
    [Header("Aesthetics")]
    
    [Tooltip("Brightness value when entity is damaged. 0f to keep original color, 1f for full white.")]
    [Range(0f, 1f)]
    public float damagedBrightness = 0.2f;
    
    [Tooltip("Duration of brightness effect when entity is damaged (s)")]
    [Range(0f, 0.2f)]
    public float damagedBrightnessDuration = 0.1f;
}
