using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManagersGenerator : MonoBehaviour
{
    [Header("Assets")]
    
    [Tooltip("Session manager prefab to instantiate on Awake, if not already created")]
    public GameObject sessionManagerPrefab;
    
    
    private void Awake()
    {
        // Only instantiate manager prefab if not already present in scene
        // This allows us to place a PersistentManagersGenerator in every scene, so we can play from any scene and
        // have access to all persistent managers, but never instantiate them more than once.
        // If done correctly, persistent managers are truly the "global singletons" you'd find in other engines.
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(sessionManagerPrefab != null, this, "[PersistentManagersGenerator] Session Manager Prefab not set on {0}", this);
        #endif
        
        if (SessionManager.Instance == null)
        {
            // Instantiate manager and flag it DontDestroyOnLoad so it's actually persistent across scenes
            // This is the only way to make them persistent, so never place a persistent manager prefab instance
            // manually in the scene! It would prevent creation of a correct, persistent instance in the same scene,
            // only to be destroyed on next Load Scene.
            DontDestroyOnLoad(Instantiate(sessionManagerPrefab));
        }
    }
}
