using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Health data component
public class Health : MonoBehaviour
{
    [Header("Parameters")]
    
    [ReadOnlyField, Tooltip("Max health value")]
    public int maxValue;
    
    
    [Header("State")]
    
    [ReadOnlyField, Tooltip("Health value")]
    public int value;
}
