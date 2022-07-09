using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Place a Persistent Managers Generator game object with this component on each scene
/// It will spawn managers that work across all scenes.
/// Some really need to live across scenes to convey information (Session Manager),
/// others are simply needed by every scene and it's more convenient to generate them once and for all than place them
/// manually in every scene, although they don't convey information across scenes (Constants Manager).
/// SEO: before other managers (that may use the instantiated persistent managers)
/// We also recommend SEO before the persistent managers themselves so that if some persistent managers are incorrectly
/// placed in the scene, instantiation from this component always has priority, then the scene managers will try to
/// initialize, notice that some instance already exists, and they will self-destruct with a warning, which is better
/// than registering the manager already in the scene (which is not DontDestroyOnLoad) and *not* instantiating
/// the DontDestroyOnLoad version at all, which is a silent failure, and may cause issues when loading a new scene
/// (it would recreate another persistent manager in the worst case, but it needed to convey information across
/// scenes, then this won't work).
public class PersistentManagersGenerator : MonoBehaviour
{
    [Header("Assets")]

    [Tooltip("Constants manager prefab to instantiate on Awake, if not already created")]
    public GameObject constantsManagerPrefab;

    [Tooltip("Session manager prefab to instantiate on Awake, if not already created")]
    public GameObject sessionManagerPrefab;


    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(sessionManagerPrefab != null, this, "[PersistentManagersGenerator] Session Manager Prefab not set on {0}", this);
        Debug.AssertFormat(constantsManagerPrefab != null, this, "[PersistentManagersGenerator] Constants Manager Prefab not set on {0}", this);
        #endif

        // Only instantiate manager prefab if not already present in scene
        // This allows us to place a PersistentManagersGenerator in every scene, so we can play from any scene in the
        // editor, and the scene we play from will instantiate all persistent managers; while further scenes won't
        // instantiate any more of them.
        // If done correctly, persistent managers are truly the "global singletons" you'd find in other engines.

        // For each persistent manager:
        // - check that persistent managers are not instantiated yet (i.e. we have just loaded the first scene to play)
        // - instantiate manager and flag it DontDestroyOnLoad so it's actually persistent across scenes

        if (ConstantsManager.Instance == null)
        {
            DontDestroyOnLoad(Instantiate(constantsManagerPrefab));
        }
        if (SessionManager.Instance == null)
        {
            DontDestroyOnLoad(Instantiate(sessionManagerPrefab));
        }
    }
}
