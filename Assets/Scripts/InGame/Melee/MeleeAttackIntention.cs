using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Melee Attack intention data component
public class MeleeAttackIntention : MonoBehaviour
{
    [ReadOnlyField, Tooltip("Does the character want to start melee attack?")] 
    public bool startAttack;
}
