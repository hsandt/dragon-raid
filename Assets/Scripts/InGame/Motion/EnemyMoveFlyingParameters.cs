using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Move parameters for flying enemies
[CreateAssetMenu(fileName = "EnemyMoveFlyingParameters", menuName = "Data/Enemy Move Flying Parameters")]
public class EnemyMoveFlyingParameters : ScriptableObject
{
    [Tooltip("Type of moving path (Linear: straight line, Wave: sinusoidal wave, " +
             "Linear Dive: straight line, then change direction when target is in range)")]
    public MovePathType movePathType = MovePathType.Linear;
    
    
    // We don't have a custom editor to gray out / hide irrelevant parameters (those not used with the current
    // move path type), so we rely on headers to indicate to the designer which fields are relevant to current
    // move path type.
    
    [Header("Linear motion parameters")]
    
    [Tooltip("Maximum speed for Linear motion and Linear Dive motion 1st part (m/s)")]
    [Range(0f, 8f)]
    public float linearMaxSpeed = 4f;
    
    
    [Header("Wave motion parameters")]
    
    [Tooltip("Horizontal speed, independent of vertical motion (m/s). Enemy always goes toward left.")]
    [Range(0f, 8f)]
    public float waveHorizontalSpeed = 4f;
    
    [Tooltip("Half-height of wave = amplitude of sinusoidal function (m)")]
    [Range(0f, 4f)]
    public float waveHalfHeight = 2f;
    
    [Tooltip("Period of sinusoidal pattern (s)")]
    [Range(0f, 6f)]
    public float wavePeriod = 3f;

    
    [Header("Linear Dive motion parameters")]
    
    [Tooltip("Dive angle, counted from enemy forward (world left), CCW (CW if negative) (degrees)")]
    [Range(-180f, 180f)]
    public float diveAngle = 90;
    
    [Tooltip("Dive speed")]
    [Range(0f, 8f)]
    public float diveSpeed = 4f;
}
