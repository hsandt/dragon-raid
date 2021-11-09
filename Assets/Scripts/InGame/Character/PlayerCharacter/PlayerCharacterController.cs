using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

public class PlayerCharacterController : ClearableBehaviour
{
    [Header("Parameters data")]

    [Tooltip("Move Parameters Data")]
    public PlayerMoveFlyingParameters playerMoveFlyingParameters;
    
    
    /* Sibling components */
    
    private MoveFlyingIntention m_MoveFlyingIntention;
    private ShootIntention m_ShootIntention;
    private MeleeAttackIntention m_MeleeAttackIntention;
    
    
    private void Awake()
    {
        m_MoveFlyingIntention = this.GetComponentOrFail<MoveFlyingIntention>();
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();
        m_MeleeAttackIntention = this.GetComponentOrFail<MeleeAttackIntention>();
    }
    
    public override void Setup()
    {
        m_MoveFlyingIntention.moveVelocity = Vector2.zero;
    
        m_ShootIntention.holdFire = false;
        m_ShootIntention.fireDirections.Clear();
        
        m_MeleeAttackIntention.startAttack = false;
    }

    /// PlayerInput action message callback for Move, called via InGameInputManager
    public void OnMove(Vector2 moveInput)
    {
        // Binary Cardinal Processor makes sure that move input is -1/0/+1 on each axis
        m_MoveFlyingIntention.moveVelocity = playerMoveFlyingParameters.maxSpeed * moveInput;
    }

    /// PlayerInput action message callback for Fire, called via InGameInputManager
    public void OnFire(bool fireInput)
    {
        // When holding fire, do not Add fire directions to the intention.
        // Instead, Shoot will compute fire direction live for each shot.     
        m_ShootIntention.holdFire = fireInput;
    }
    
    /// PlayerInput action message callback for Melee Attack, called via InGameInputManager
    public void OnMeleeAttack()
    {
        m_MeleeAttackIntention.startAttack = true;
    }
}
