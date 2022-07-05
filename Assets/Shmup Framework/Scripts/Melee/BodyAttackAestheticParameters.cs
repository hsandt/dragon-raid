using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Aesthetic parameters for BodyAttack
[CreateAssetMenu(fileName = "BodyAttackAestheticParameters", menuName = "Data/Body Attack Aesthetic Parameters")]
public class BodyAttackAestheticParameters : ScriptableObject
{
    [Tooltip("SFX played on hit")]
    public AudioClip sfxHit;
}
