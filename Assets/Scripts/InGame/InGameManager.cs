using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityEngine;

using UnityConstants;
using CommonsPattern;
using UnityEngine.SceneManagement;

/// In-game manager
/// SEO:
/// - after LocatorManager (for this Init)
/// - before PlayerCharacterPoolManager and EnemyPoolManager (for their Init, which spawn various components
///   that may rely on InGameManager for asset reference caching on Awake)
public class InGameManager : SingletonManager<InGameManager>
{
    [Header("Assets")]
    
    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;
    
    [Tooltip("Health Shared Parameters asset")]
    public HealthSharedParameters healthSharedParameters;
    
    
    /* Cached scene references */
    
    /// Cached player spawn transform
    private Transform m_PlayerSpawnTransform;

    /// Cached level data, retrieved from the level identifier of the current scene
    private LevelData m_LevelData;
    
    /// Cached level data, retrieved from the level identifier of the current scene (getter)
    public LevelData LevelData => m_LevelData;

    /// Player Character Master component, reference stored after spawn
    private CharacterMaster m_PlayerCharacterMaster;
    
    /// Player Character Master component, reference stored after spawn (getter)
    public CharacterMaster PlayerCharacterMaster => m_PlayerCharacterMaster;
    
    
    /* State */

    /// Time elapsed since level start
    private float m_TimeSinceLevelStart;
    
    /// True iff level finish sequence is playing
    private bool m_IsFinishingLevel;

    protected override void Init()
    {
        base.Init();

        // Find player character spawn position
        m_PlayerSpawnTransform = LocatorManager.Instance.FindWithTag(Tags.PlayerSpawnPosition)?.transform;

        m_LevelData = LocatorManager.Instance.FindWithTag(Tags.LevelIdentifier)?.GetComponent<LevelIdentifier>()?.levelData;
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(levelDataList != null, "No Level Data List asset set on InGame Manager", this);
        Debug.Assert(healthSharedParameters != null, "No Health Shared Parameters asset set on InGame Manager", this);
        Debug.AssertFormat(m_PlayerSpawnTransform != null, this,
            "[InGameManager] No active object with tag PlayerSpawnPosition found in scene");
        Debug.AssertFormat(m_LevelData != null, this,
            "[InGameManager] Could not find active LevelIdentifier object > LevelIdentifier component > Level Data");
        #endif
    }
    
    private void Start()
    {
        // Setup level
        SetupLevel();
    }

    private void SetupLevel()
    {
        m_IsFinishingLevel = false;

        ScrollingManager.Instance.Setup();
        SpatialEventManager.Instance.Setup();
        EnemyWaveManager.Instance.Setup();
        
        if (m_LevelData != null)
        {
            MusicManager.Instance.PlayBgm(m_LevelData.bgm);        
        }
        
        SpawnPlayerCharacter();
    }
    
    private void SpawnPlayerCharacter()
    {
        // Spawn Player Character
        if (m_PlayerSpawnTransform != null)
        {
            // Spawn character as a pooled object (in a pool of 1 object)
            m_PlayerCharacterMaster = PlayerCharacterPoolManager.Instance.SpawnCharacter(m_PlayerSpawnTransform.position);
            
            // Assign HUD's player health gauge to player health system (on Restart, it only refreshes the gauge)
            var healthSystem = m_PlayerCharacterMaster.GetComponentOrFail<HealthSystem>();
            HUD.Instance.AssignGaugeHealthPlayerTo(healthSystem);
        }
    }

    private void ClearLevel()
    {
        // Despawn the player character
        
        // First clean up references to avoid relying on Unity's "destroyed null"
        // Do not clear cached scene references m_PlayerSpawnTransform and m_LevelData yet,
        // those can only be destroyed when loading a new scene (assuming InGameManager is flagged DontDestroyOnLoad
        // so it preserves the destroyed reference), so we'll handle this in FinishLevel
        m_PlayerCharacterMaster = null;
        
        // We use the generic Pool API to release all objects, but it really only releases 1 here
        PlayerCharacterPoolManager.Instance.ReleaseAllObjects();

        // Despawn all enemies
        EnemyPoolManager.Instance.ReleaseAllObjects();
        
        // Clean up all projectiles
        ProjectilePoolManager.Instance.ReleaseAllObjects();
        
        // Clean up all FX
        FXPoolManager.Instance.ReleaseAllObjects();
        
        // Note: we're now restarting Music nor stopping all SFX
    }

    public void RestartLevel()
    {
        // Don't allow level restart during finish sequence to avoid invalid states likelLoading next level async
        // but restarting this level in the middle
        if (!m_IsFinishingLevel)
        {
            ClearLevel();
            SetupLevel();
        }
    }

    public void FinishLevel()
    {
        if (!m_IsFinishingLevel)
        {
            m_IsFinishingLevel = true;
            
            // Disable all other singleton managers that have an update to avoid weird things happening during the
            // Finish Level sequence.
            ScrollingManager.Instance.enabled = false;

            // If InGameManager is flagged DontDestroyOnLoad, it will be kept in next level (if any),
            // and it wil lbe cleaner to clean the cached scene references first.
            // But we'll also need to set those again after loading the new scene.
            // Make sure to store current level index first.
            int currentLevelIndex = m_LevelData.levelIndex;
            m_PlayerSpawnTransform = null;
            m_LevelData = null;

            // first, do a brutal sync load
            if (currentLevelIndex < levelDataList.levelDataArray.Length - 1)
            {
                // we are not in the last level yet, proceed to next level
                int nextLevelIndex = currentLevelIndex + 1;
                LevelData nextLevelData = levelDataList.levelDataArray[nextLevelIndex];
                if (nextLevelData != null)
                {
                    int nextLevelSceneBuildIndex = (int)nextLevelData.sceneEnum;
                    
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.AssertFormat(nextLevelSceneBuildIndex == nextLevelIndex + 1, nextLevelData,
                        "[InGameManager] FinishLevel: next level scene build index ({0}) is not next level index + 1 ({1}), " +
                        "where offset 1 represents the Title scene. Did you add another non-level scene before " +
                        "the level scenes, causing ScenesEnum to offset all level scene build indices?",
                        nextLevelSceneBuildIndex, nextLevelIndex + 1);
                    #endif
                    
                    SceneManager.LoadScene(nextLevelSceneBuildIndex);
                }
                else
                {
                    Debug.LogErrorFormat(levelDataList, "[InGameManager] FinishLevel: missing level data for " +
                        "next level at index {0} in levelDataList. Falling back to Title scene.", nextLevelIndex);
                    SceneManager.LoadScene((int)ScenesEnum.Title);
                }
            }
            else
            {
                SceneManager.LoadScene((int)ScenesEnum.Title);
            }
        }
    }
}
