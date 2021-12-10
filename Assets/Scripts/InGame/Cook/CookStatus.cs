using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Cook status data component
public class CookStatus : MonoBehaviour
{
    [Header("Parameters")]
    
    [ReadOnlyField, Tooltip("Max cook progress. When reached, enemy is fully cooked")]
    public int maxCookProgress;
    
    
    [Header("State")]
    
    [ReadOnlyField, Tooltip("Cook progress")]
    public int cookProgress;
}
