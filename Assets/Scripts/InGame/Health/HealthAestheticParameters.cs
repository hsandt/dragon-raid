using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Aesthetic parameters for Health System
[CreateAssetMenu(fileName = "HealthAestheticParameters", menuName = "Data/Health Aesthetic Parameters")]
public class HealthAestheticParameters : ScriptableObject
{
    [Tooltip("SFX played on death")]
    public AudioClip sfxDeath;
}
