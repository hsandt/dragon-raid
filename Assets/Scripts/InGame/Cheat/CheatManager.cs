using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsPattern;

public class CheatManager : SingletonManager<CheatManager>
{
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    
    [SerializeField, Tooltip("Scrolling to immediately warp to on Start")]
    private float initialScrolling = 0f;

    public void OnLevelSetup()
    {
        ScrollingManager.Instance.CheatAdvanceScrolling(initialScrolling);
    }

    private void Update()
    {
        // Player Input components cannot share the same device (here: keyboard),
        // so it's simpler to hardcode cheat keys than reusing the Dragon Player Input for cheats 
        
        // Key map:
        // - press R to restart level
        // - press F to finish level
        // - press K to kill all spawned enemies
        
        // if (Keyboard.current.rKey.wasPressedThisFrame)
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (InGameManager.Instance.CanRestartLevel)
            {
                InGameManager.Instance.RestartLevel();
            }
        }
        else if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (InGameManager.Instance.CanFinishLevel)
            {
                InGameManager.Instance.FinishLevel();
            }
        }
        else if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            if (InGameManager.Instance.CanUseCheat)
            {
                EnemyPoolManager.Instance.KillAllEnemies();
            }
        }
        
        // HACK to fix bug causing Linux Editor to make those keys considered press
        // when first pressing any key since entering Play mode, causing unwanted effects
        if (Keyboard.current.rKey.wasReleasedThisFrame)
        {
        }
        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
        }
        if (Keyboard.current.kKey.wasReleasedThisFrame)
        {
        }
    }
    
    #endif
}
