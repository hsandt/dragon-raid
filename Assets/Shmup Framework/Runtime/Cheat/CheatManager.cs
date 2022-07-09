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
    
    #endif
}
