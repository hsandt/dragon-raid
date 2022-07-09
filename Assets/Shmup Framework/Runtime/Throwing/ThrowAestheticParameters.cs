using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Aesthetic parameters for Throw
[CreateAssetMenu(fileName = "ThrowAestheticParameters", menuName = "Data/Throw Aesthetic Parameters")]
public class ThrowAestheticParameters : ScriptableObject
{
    [Tooltip("SFX played on projectile spawn")]
    public AudioClip sfxSpawnProjectile;
}
