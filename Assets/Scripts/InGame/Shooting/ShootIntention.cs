using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Shoot intention data component
public class ShootIntention : MonoBehaviour
{
    [ReadOnlyField, Tooltip("Does the character want to keep firing? (cooldown still applies, causing periodical fire)")] 
    public bool holdFire;
}
