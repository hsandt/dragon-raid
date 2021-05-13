using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Projectile Visual parameters for Projectile
[CreateAssetMenu(fileName = "ProjectileAestheticParameters", menuName = "Data/Projectile Aesthetic Parameters")]
public class ProjectileAestheticParameters : ScriptableObject
{
    [Tooltip("Name of the FX played on projectile impact")]
    public string fxName = "Undefined";
    
    [Tooltip("SFX played on projectile spawn")]
    public AudioClip sfxSpawn;
    
    [Tooltip("SFX played on projectile impact")]
    public AudioClip sfxImpact;
}
