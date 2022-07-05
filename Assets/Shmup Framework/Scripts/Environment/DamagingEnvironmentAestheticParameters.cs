using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Aesthetic parameters for Damaging Environment
[CreateAssetMenu(fileName = "DamagingEnvironmentAestheticParameters", menuName = "Data/Damaging Environment Aesthetic Parameters")]
public class DamagingEnvironmentAestheticParameters : ScriptableObject
{
    [Tooltip("SFX played on periodic damage")]
    public AudioClip sfxDamage;
}
