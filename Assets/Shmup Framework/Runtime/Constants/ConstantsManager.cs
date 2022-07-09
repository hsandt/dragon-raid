using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using CommonsDebug;
using CommonsPattern;

/// Manager that finds, caches and provides access to Project Constants (Tags and Layers)
/// It replaces Unity Constants when we are working on non-project-specific code such as framework code,
/// where we cannot access the generated UnityConstants.cs, which is project-specific.
/// Runtime scripts should use shortcut accessors Tags, Layers and Scenes for quick access,
/// as they retrieve the cached Constants asset under the hood, and offer an interface similar to UnityConstants.
/// Ex: UnityConstants.Tags.MyTag -> ConstantsManager.Tags.MyTag
/// Editor scripts should use static method GetOrCreateConstants since it cannot
/// benefit from caching, since the Manager component only exists on a scene game object when game is running.
/// Ex: UnityConstants.Tags.MyTag -> `Constants constants = ConstantsManager.GetOrCreateConstants();` then use constants.Tags.MyTag
/// SEO: before any script whose Awake uses ConstantsManager to access Tags, Layers or Scenes
/// Just like LocatorManager, it is preferable to initialize it very early, before even other managers,
/// so executionOrder was set to -80 in .meta to work with most scripts
/// However, if you're using the PersistentManagersGenerator, then it will be instantiated via
/// PersistentManagersGenerator, and what matters is that the SEO of the latter is before other managers.
public class ConstantsManager : SingletonManager<ConstantsManager>
{
    /// Path to expected Constants asset inside some Resources folder, without ".asset"
    /// (important to work with Resources.Load)
    private const string constantsPathInResources = "Shmup Framework/Constants";

    /// Path to Resources folder where new Constants asset will be created if no Constants asset is found
    private const string defaultResourcesDirectoryPath = "Resources";


    /* Cached asset references */

    private Constants m_CachedConstants;


    /* Shortcut accessors (runtime only!) */

    public static Constants.TagsMapping Tags => Instance.m_CachedConstants.Tags;
    public static Constants.LayersMapping Layers => Instance.m_CachedConstants.Layers;
    public static Constants.ScenesMapping Scenes => Instance.m_CachedConstants.Scenes;


    public static Constants GetConstants()
    {
        return Resources.Load<Constants>(constantsPathInResources);
    }

    #if UNITY_EDITOR
    /// Get Constants from Resources, or create one at default location if there is none
    /// This is editor-only, as only editor should be able to create new assets
    public static Constants GetOrCreateConstants()
    {
        // Load Constants from Resources, if any
        // (we are in the editor, so don't rely on m_CachedConstants which only works at runtime)
        Constants constants = GetConstants();

        if (constants == null)
        {
            string newConstantsPath = Path.Combine("Assets", defaultResourcesDirectoryPath, constantsPathInResources);

            // create directory recursively if it doesn't exist yet
            string newConstantsDirectory = Path.GetDirectoryName(newConstantsPath);
            if (!Directory.Exists(newConstantsDirectory))
            {
                Directory.CreateDirectory(newConstantsDirectory);
            }

            // create missing Constants asset
            constants = ScriptableObject.CreateInstance<Constants>();

            // AssetDatabase.CreateAsset needs extension .asset to create with correct file name
            AssetDatabase.CreateAsset(constants, $"{newConstantsPath}.asset");

            Debug.Log($"[ConstantsManager] No Constants asset found at any Resources/{constantsPathInResources}. Creating one at {newConstantsPath}.",
                constants);
        }

        return constants;
    }
    #endif

    protected override void Init()
    {
        // Cache constants for runtime usage
        m_CachedConstants = GetConstants();
        DebugUtil.AssertFormat(m_CachedConstants != null,
            "[ConstantsManager] Init: Could not find Constants asset at Resources/{0}",
            constantsPathInResources);
    }
}
