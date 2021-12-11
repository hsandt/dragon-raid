using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Pick-up parameters for PickUp System
[CreateAssetMenu(fileName = "PickUpParameters", menuName = "Data/Pick-up Parameters")]
public class PickUpParameters : ScriptableObject
{
    [Tooltip("Health recovered when picking this item")]
    public int healthRecovery = 1;
}