using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// Player Input controller script to handle system input
/// When player becomes keyboard/gamepad user via the player character, no other entity can handle it,
/// so we cannot put system input (e.g. TogglePauseMenu) handling on another game object than the player character.  
public class PlayerSystemController : MonoBehaviour
{
    /// PlayerInput action message callback for TogglePauseMenu
    private void OnTogglePauseMenu(InputValue value)
    {
        // We assume TogglePauseMenu action is only bound to press, so no need to check value.isPressed
        InGameManager.Instance.TryTogglePauseMenu();
    }
}
