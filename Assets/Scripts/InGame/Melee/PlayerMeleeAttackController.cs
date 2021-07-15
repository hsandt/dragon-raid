using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// System for MeleeAttackIntention on Player character: handles control
/// SEO: before MeleeAttack
public class PlayerMeleeAttackController : BaseMeleeAttackController
{
    /// PlayerInput action message callback for Melee Attack
    private void OnMeleeAttack(InputValue value)
    {
        m_MeleeAttackIntention.startAttack = value.isPressed;
    }
}
