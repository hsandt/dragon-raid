using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityEngine;

using UnityConstants;
using CommonsPattern;
using UnityEngine.SceneManagement;

public class InGameManager : SingletonManager<InGameManager>
{
    [Header("Assets")]
    
    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;
    
    
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

    /// True iff level finish sequence is playing
    private bool m_IsFinishingLevel;

    protected override void Init()
    {
        base.Init();

        // Find player character spawn position
        m_PlayerSpawnTransform = LocatorManager.Instance.FindWithTag(Tags.PlayerSpawnPosition)?.transform;
        Debug.AssertFormat(m_PlayerSpawnTransform != null, this,
            "[InGameManager] No active object with tag PlayerSpawnPosition found in scene");

        m_LevelData = LocatorManager.Instance.FindWithTag(Tags.LevelIdentifier)?.GetComponent<LevelIdentifier>()
            ?.levelData;
        Debug.AssertFormat(m_LevelData != null, this,
            "[InGameManager] Could not find active LevelIdentifier object > LevelIdentifier component > Level Data");
    }
    
    private void Start()
    {
        // Setup level
        SetupLevel();
    }

    private void SetupLevel()
    {
        m_IsFinishingLevel = false;
        
        SpawnPlayerCharacter();
        EnemyWaveManager.Instance.Setup();
        
        if (m_LevelData != null)
        {
            MusicManager.Instance.PlayBgm(m_LevelData.bgm);        
        }
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
        m_PlayerCharacterMaster = null;
        m_LevelData = null;
        
        // We use the generic Pool API to release all objects, but it really only releases 1 here
        PlayerCharacterPoolManager.Instance.ReleaseAllObjects();

        // Despawn all enemies
        EnemyPoolManager.Instance.ReleaseAllObjects();
        
        // Clean up all projectiles
        ProjectilePoolManager.Instance.ReleaseAllObjects();
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

            // first, do a brutal sync load
            if (m_LevelData.levelIndex < levelDataList.levelDataArray.Length - 1)
            {
                // we are not in the last level yet, proceed to next level
                int nextLevelIndex = m_LevelData.levelIndex + 1;
                LevelData nextLevelData = levelDataList.levelDataArray[nextLevelIndex];
                if (nextLevelData != null)
                {
                    int nextLevelSceneBuildIndex = (int)nextLevelData.sceneEnum;
                    Debug.AssertFormat(nextLevelSceneBuildIndex == nextLevelIndex + 1, nextLevelData,
                        "[InGameManager] FinishLevel: next level scene build index ({0}) is not next level index + 1 ({1}), " +
                        "where offset 1 represents the Title scene. Did you add another non-level scene before " +
                        "the level scenes, causing ScenesEnum to offset all level scene build indices?",
                        nextLevelSceneBuildIndex, nextLevelIndex + 1);
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
