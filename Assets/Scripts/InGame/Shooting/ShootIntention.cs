using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Shoot intention data component
public class ShootIntention : MonoBehaviour
{
    [ReadOnlyField, Tooltip("Intended fire flag (must be consumed)")] 
    public bool fire;
}
