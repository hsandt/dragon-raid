using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Aesthetic parameters for MeleeAttack
[CreateAssetMenu(fileName = "MeleeAttackAestheticParameters", menuName = "Data/Melee Attack Aesthetic Parameters")]
public class MeleeAttackAestheticParameters : ScriptableObject
{
    [Tooltip("SFX played on attack (whether it hits on not)")]
    public AudioClip sfxAttack;
    
    [Tooltip("SFX played on hit (stacks with attack SFX)")]
    public AudioClip sfxHit;
}
