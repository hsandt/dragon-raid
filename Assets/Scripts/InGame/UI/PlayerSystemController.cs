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
        // ! This won't be called while Player Character object is inactive, as the Player Input and this component
        // is on it. If you really need to be able to open the pause menu between time of death and respawn/restart,
        // then consider just hiding the PC in PlayerCharacterMaster on Release instead.
        // Or move this input handling somewhere else, but you'll have to move PC input handling too,
        // and redirect it to the PC (because only one Player Input can be assigned a device).
        
        // We assume TogglePauseMenu action is only bound to press, so no need to check value.isPressed
        InGameManager.Instance.TryTogglePauseMenu();
    }
}
