using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    private void Update()
    {
        // Player Input components cannot share the same device (here: keyboard),
        // so it's simpler to hardcode cheat keys than reusing the Dragon Player Input for cheats 
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            InGameManager.Instance.RestartLevel();
        }
    }
}
