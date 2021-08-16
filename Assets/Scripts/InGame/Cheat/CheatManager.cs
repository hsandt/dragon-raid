using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void Update()
    {
        // Player Input components cannot share the same device (here: keyboard),
        // so it's simpler to hardcode cheat keys than reusing the Dragon Player Input for cheats 
        
        // Key map:
        // - press R to restart level
        // - press F to finish level
        // - press K to kill all spawned enemies
        
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            InGameManager.Instance.RestartLevel();
        }
        else if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            InGameManager.Instance.FinishLevel();
        }
        else if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            EnemyPoolManager.Instance.KillAllEnemies();
        }
    }
    #endif
}
