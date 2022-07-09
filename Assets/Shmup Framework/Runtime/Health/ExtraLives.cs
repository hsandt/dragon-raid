using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// ExtraLives data component
public class ExtraLives : MonoBehaviour
{
    [Header("Parameters")]
    
    [ReadOnlyField, Tooltip("Max extra lives count")]
    public int maxCount;
    
    
    [Header("State")]
    
    [ReadOnlyField, Tooltip("Extra lives count")]
    public int count;
}
