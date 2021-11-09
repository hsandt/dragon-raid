using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// Intermediate script that buffers input and delegates input to Player Character Controller when active,
/// while handling System input directly at all times
/// SEO: before any action script processing intention (Move, Shoot, MeleeAttack), like other control scripts
/// Right before InGameManager is a good place
public class InGameInputManager : SingletonManager<InGameInputManager>
{
    /// Controller script of Player Character input callbacks will be delegated to
    private PlayerCharacterController m_PlayerCharacterController;
    
    
    /* State */

    // Inputs are buffered at all times, even when player character is inactive,
    // so that we can resume activity with the correct values when it's active again.
    // Indeed, input messages are minimalistic and only sent on input change,
    // so holding the same move direction or fire button will not send another event
    // after player character has respawned, and Setup has cleared intention.
    
    private Vector2 m_MoveInput;
    private bool m_FireInput;
    private bool m_MeleeAttackInput;
    

    public void SetPlayerCharacterController(PlayerCharacterController playerCharacterController)
    {
        if (m_PlayerCharacterController != playerCharacterController)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (m_PlayerCharacterController != null && playerCharacterController != null)
            {
                Debug.LogWarningFormat(this,
                    "[InGameInputManager] Player Character Controller already set on {0} to {1}, " +
                    "it will be overwritten with {2}", this, m_PlayerCharacterController, playerCharacterController);
            }
            #endif
            
            m_PlayerCharacterController = playerCharacterController;
        }
    }
    
    /// PlayerInput action message callback for TogglePauseMenu
    /// This is always accessible, even when Player Character is not active
    private void OnTogglePauseMenu(InputValue value)
    {
        // Immediately toggle pause menu
        // We assume TogglePauseMenu action is only bound to press, so no need to check value.isPressed
        InGameManager.Instance.TryTogglePauseMenu();
    }
    
    /// PlayerInput action message callback for Move
    private void OnMove(InputValue value)
    {
        m_MoveInput = value.Get<Vector2>();
    }
    
    /// PlayerInput action message callback for Fire
    private void OnFire(InputValue value)
    {
        m_FireInput = value.isPressed;
    }
    
    /// PlayerInput action message callback for Melee Attack
    private void OnMeleeAttack(InputValue value)
    {
        m_MeleeAttackInput = value.isPressed;
    }

    private void FixedUpdate()
    {
        // Consume Press input each frame (never disable this script, even on Pause)
        // to avoid sticky input while Player Character is inactive that may cause unwanted action
        // when it's active again (it is generally safe as Player Character Controller Setup will clear them,
        // but cleaner not to rely on this)
        bool consumedMeleeAttackInput = ControlUtil.ConsumeBool(ref m_MeleeAttackInput);

        if (m_PlayerCharacterController != null)
        {
            // Continuous and Press/Release inputs are directly sent
            m_PlayerCharacterController.OnMove(m_MoveInput);
            m_PlayerCharacterController.OnFire(m_FireInput);
            
            if (consumedMeleeAttackInput)
            {
                m_PlayerCharacterController.OnMeleeAttack();
            }
        }
    }
    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    /// PlayerInput action message callback for Cheat_Restart
    /// This is always accessible, even when Player Character is not active
    private void OnCheat_Restart(InputValue value)
    {
        if (InGameManager.Instance.CanRestartLevel)
        {
            InGameManager.Instance.RestartLevel();
        }
    }

    /// PlayerInput action message callback for Cheat_FinishLevel
    /// This is always accessible, even when Player Character is not active
    private void OnCheat_FinishLevel(InputValue value)
    {
        if (InGameManager.Instance.CanFinishLevel)
        {
            InGameManager.Instance.FinishLevel();
        }
    }

    /// PlayerInput action message callback for Cheat_KillAllEnemies
    /// This is always accessible, even when Player Character is not active
    private void OnCheat_KillAllEnemies(InputValue value)
    {
        if (InGameManager.Instance.CanUseCheat)
        {
            EnemyPoolManager.Instance.KillAllEnemies();
        }
    }

    /// PlayerInput action message callback for Cheat_KillPlayerCharacter
    /// This is always accessible, but obviously does nothing when Player Character is inactive
    private void OnCheat_KillPlayerCharacter(InputValue value)
    {
        if (InGameManager.Instance.CanUseCheat)
        {
            PlayerCharacterPoolManager.Instance.KillPlayerCharacter();
        }
    }

    #endif
}
