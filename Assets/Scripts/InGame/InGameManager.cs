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
    
    [Tooltip("In-game Flow Parameters asset")]
    public InGameFlowParameters inGameFlowParameters;
    
    [Tooltip("Health Shared Parameters asset (only used by other scripts)")]
    public HealthSharedParameters healthSharedParameters;
    
    
    /* Cached scene references */
    
    /// Cached player spawn transform
    private Transform m_PlayerSpawnTransform;

    /// Cached level data, retrieved from the level identifier of the current scene
    private LevelData m_LevelData;
    
    /// Cached level data, retrieved from the level identifier of the current scene (getter)
    public LevelData LevelData => m_LevelData;

    /// Player Character Master component, reference stored after spawn
    private PlayerCharacterMaster m_PlayerCharacterMaster;
    
    /// Player Character Master component, reference stored after spawn (getter)
    public PlayerCharacterMaster PlayerCharacterMaster => m_PlayerCharacterMaster;
    
    /// Cached canvas pause menu reference
    public CanvasPauseMenu m_CanvasPauseMenu;
    
    /// Cached canvas level reference
    public CanvasLevel m_CanvasLevel;
    
    
    /* State */

    /// Time elapsed since level start
    private float m_TimeSinceLevelStart;
    
    /// True iff game is paused / pause menu is open
    private bool m_IsGamePaused;

    /// True iff level finish sequence is playing
    private bool m_IsFinishingLevel;

    public bool CanPauseGame => !m_IsGamePaused && !m_IsFinishingLevel;
    public bool CanRestartLevel => !m_IsGamePaused && !m_IsFinishingLevel;
    public bool CanFinishLevel => !m_IsGamePaused && !m_IsFinishingLevel;
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    public bool CanUseCheat => !m_IsGamePaused && !m_IsFinishingLevel;
    #endif

    protected override void Init()
    {
        base.Init();

        // Find player character spawn position
        m_PlayerSpawnTransform = LocatorManager.Instance.FindWithTag(Tags.PlayerSpawnPosition)?.transform;

        m_LevelData = LocatorManager.Instance.FindWithTag(Tags.LevelIdentifier)?.GetComponent<LevelIdentifier>()?.levelData;
        
        m_CanvasPauseMenu = LocatorManager.Instance.FindWithTag(Tags.CanvasPauseMenu)?.GetComponent<CanvasPauseMenu>();
        m_CanvasLevel = LocatorManager.Instance.FindWithTag(Tags.CanvasLevel)?.GetComponent<CanvasLevel>();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(levelDataList != null, "[InGameManager] No Level Data List asset set on InGame Manager", this);
        Debug.Assert(inGameFlowParameters != null, "[InGameManager] No In-game Flow Parameters asset set on InGame Manager", this);
        Debug.Assert(healthSharedParameters != null, "[InGameManager] No Health Shared Parameters asset set on InGame Manager", this);
        Debug.Assert(m_PlayerSpawnTransform != null, "[InGameManager] No active object with tag PlayerSpawnPosition found in scene", this);
        Debug.Assert(m_LevelData != null, "[InGameManager] Could not find active LevelIdentifier object > LevelIdentifier component > Level Data", this);
        Debug.Assert(m_CanvasPauseMenu != null, "[InGameManager] Could not find active Canvas Pause Menu object > CanvasPauseMenu component", this);
        Debug.Assert(m_CanvasLevel != null, "[InGameManager] Could not find active Canvas Level object > CanvasLevel component", this);
        #endif
    }
    
    private IEnumerator Start()
    {
        // If playing in the editor directly into Level scene, do some basic setup
        // so SessionManager knows what we're doing, in case it needs this info at some point
        SessionManager.Instance.EnterFallbackModeIfNone(m_LevelData.levelIndex);
        
        // Hide pause menu (don't use Hide, which may have an animation later)
        m_CanvasPauseMenu.gameObject.SetActive(false);
        
        // Hide performance assessment until we finish the level
        PerformanceAssessment.Instance.Hide();
        
        // Setup level
        SetupLevel();

        yield return m_CanvasLevel.FadeIn(inGameFlowParameters.fadeInDuration);
    }

    private void SetupLevel()
    {
        m_IsGamePaused = false;
        m_IsFinishingLevel = false;

        ScrollingManager.Instance.Setup();
        SpatialEventManager.Instance.Setup();
        EnemyWaveManager.Instance.Setup();
        
        if (m_LevelData != null)
        {
            MusicManager.Instance.PlayBgm(m_LevelData.bgm);        
        }
        
        InitialSpawnPlayerCharacter();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (CheatManager.Instance)
        {
            CheatManager.Instance.OnLevelSetup();
        }
        #endif
    }
    
    private void InitialSpawnPlayerCharacter()
    {
        // Spawn Player Character
        if (m_PlayerSpawnTransform != null)
        {
            // Spawn character as a pooled object (in a pool of 1 object)
            m_PlayerCharacterMaster = PlayerCharacterPoolManager.Instance.SpawnCharacter(m_PlayerSpawnTransform.position);
            
            // Assign HUD's player health gauge to player health system (on Restart, it only refreshes the gauge)
            var healthSystem = m_PlayerCharacterMaster.GetComponentOrFail<HealthSystem>();
            HUD.Instance.AssignGaugeHealthPlayerTo(healthSystem);
            
            // Same thing for extra lives
            var extraLivesSystem = m_PlayerCharacterMaster.GetComponentOrFail<ExtraLivesSystem>();
            extraLivesSystem.InitExtraLives();
            HUD.Instance.AssignExtraLivesViewTo(extraLivesSystem);
        }
    }

    private void RespawnPlayerCharacter()
    {
        // Respawn Player Character
        // Unlike InitialSpawnPlayerCharacter above, we don't need to assign HUD view and we don't init extra lives
        if (m_PlayerSpawnTransform != null)
        {
            // Respawn character (we keep using the same spawn position, but we could also respawn at the last position
            // before death depending on design)
            m_PlayerCharacterMaster = PlayerCharacterPoolManager.Instance.SpawnCharacter(m_PlayerSpawnTransform.position);
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
        
        EnemyWaveManager.Instance.Clear();
        
        // We use the generic Pool API to release all objects, but it really only releases 1 here
        PlayerCharacterPoolManager.Instance.ReleaseAllObjects();

        // Despawn all enemies
        // (this is a low-level method and will not trigger any death / cleared wave event)
        EnemyPoolManager.Instance.ReleaseAllObjects();
        
        // Clean up all projectiles
        ProjectilePoolManager.Instance.ReleaseAllObjects();
        
        // Clean up all FX
        FXPoolManager.Instance.ReleaseAllObjects();
        
        // Clean up all environment props
        foreach (Transform environmentPropTransform in ScrollingManager.Instance.GetMidgroundLayer().transform)
        {
            // Remember that all environment props should be pooled, directly or indirectly
            // (e.g. attached to an Enemy, itself pooled), so just deactivate it 
            environmentPropTransform.gameObject.SetActive(false);
        }
        
        // Note: we're not restarting Music nor stopping all SFX
        // We'll eventually do a proper fade-out so this should be enough to cover the duration of the longest SFX.
    }

    public void RestartLevel()
    {
        // Don't allow level restart during finish sequence to avoid invalid states likelLoading next level async
        // but restarting this level in the middle
        if (CanRestartLevel)
        {
            ClearLevel();
            SetupLevel();
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat("[InGameManager] RestartLevel: CanRestartLevel is false (m_IsFinishingLevel is {0})", m_IsFinishingLevel);
        }
        #endif
    }
    
    // TODO: add Pause/Resume here
    // Remember to disable any script with coroutines like EnemyWave so the coroutines are paused too

    public void TryTogglePauseMenu()
    {
        if (m_IsGamePaused)
        {
            m_CanvasPauseMenu.Hide();
            ResumeGame();
        }
        else
        {
            if (CanPauseGame)
            {
                m_CanvasPauseMenu.Show();
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (CanPauseGame)
        {
            m_IsGamePaused = true;
            
            ScrollingManager.Instance.Pause();
            EnemyWaveManager.Instance.Pause();
            
            // TODO: pause BGM?
    //        MusicManager.Instance.PauseBgm();
            
            PlayerCharacterPoolManager.Instance.PauseCharacter();
            EnemyPoolManager.Instance.PauseAllEnemies();
            ProjectilePoolManager.Instance.PauseAllProjectiles();
            FXPoolManager.Instance.PauseAllFX();
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat("[InGameManager] PauseGame: CanPauseGame is false (m_IsGamePaused is {0}, m_IsFinishingLevel is {1})", m_IsGamePaused, m_IsFinishingLevel);
        }
        #endif
    }
    
    public void ResumeGame()
    {
        m_IsGamePaused = false;

        ScrollingManager.Instance.Resume();
        EnemyWaveManager.Instance.Resume();
        
        // TODO: resume BGM?
//        MusicManager.Instance.ResumeBgm();
        
        PlayerCharacterPoolManager.Instance.ResumeCharacter();
        EnemyPoolManager.Instance.ResumeAllEnemies();
        ProjectilePoolManager.Instance.ResumeAllProjectiles();
        FXPoolManager.Instance.ResumeAllFX();
    }
    
    public void FinishLevel()
    {
        if (CanFinishLevel)
        {
            m_IsFinishingLevel = true;
            
            // Immediately notify session manager that level was finished
            // If play mode progress can be saved, then it is immediately auto-saved.
            // This is useful so player doesn't lose it if application is shutdown.
            // In the worst case, they'll miss the Performance Assessment screen.
            SessionManager.Instance.NotifyLevelFinished(m_LevelData.levelIndex);
            
            // Disable all other singleton managers that have an update to avoid weird things happening during the
            // Finish Level sequence.
            ScrollingManager.Instance.StopScrolling();

            StartCoroutine(FinishLevelAsync());
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat("[InGameManager] FinishLevel: CanFinishLevel is false (m_IsGamePaused is {0}, m_IsFinishingLevel is {1})", m_IsGamePaused, m_IsFinishingLevel);
        }
        #endif
    }
    
    private IEnumerator FinishLevelAsync()
    {
        yield return new WaitForSeconds(inGameFlowParameters.performanceAssessmentDelay);
        
        // Show performance assessment canvas
        PerformanceAssessment.Instance.Show();
        
        yield return new WaitForSeconds(inGameFlowParameters.loadNextLevelDelay);
        
        LoadNextLevelOrGoBackToTitle();
    }

    private void LoadNextLevelOrGoBackToTitle()
    {
        // If InGameManager is flagged DontDestroyOnLoad, it will be kept in next level (if any),
        // and it will be cleaner to clean the cached scene references first.
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
                // convert scene enum to scene build index and load next level scene
                
                int nextLevelSceneBuildIndex = (int) nextLevelData.sceneEnum;

                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.AssertFormat(nextLevelSceneBuildIndex == nextLevelIndex + 1, nextLevelData,
                    "[InGameManager] FinishLevel: next level scene build index ({0}) is not next level index + 1 ({1}), " +
                    "where offset 1 represents the Title scene. Did you add another non-level scene before " +
                    "the level scenes, causing ScenesEnum to offset all level scene build indices?",
                    nextLevelSceneBuildIndex, nextLevelIndex + 1);
                #endif

                SceneManager.LoadScene(nextLevelSceneBuildIndex);
                return;
            }
            
            Debug.LogErrorFormat(levelDataList, "[InGameManager] FinishLevel: missing level data for " +
                "next level at index {0} in levelDataList. Falling back to Title scene.",
                nextLevelIndex);
        }
        
        // last level was finished, or we failed to find next level => clear current save and go back to title
        ExitToTitleMenu();
    }

    public static void ExitToTitleMenu()
    {
        SessionManager.Instance.ExitCurrentPlayMode();
        SceneManager.LoadScene((int) ScenesEnum.Title);
    }

    public void RespawnPlayerCharacterAfterDelay()
    {
        StartCoroutine(RespawnPlayerCharacterAfterDelayAsync());
    }
    
    private IEnumerator RespawnPlayerCharacterAfterDelayAsync()
    {
        yield return new WaitForSeconds(inGameFlowParameters.respawnDelay);
        RespawnPlayerCharacter();
    }
    
    public void PlayGameOverRestart()
    {
        StartCoroutine(PlayGameOverRestartAsync());
    }
    
    private IEnumerator PlayGameOverRestartAsync()
    {
        yield return new WaitForSeconds(inGameFlowParameters.gameOverDelay);
        yield return m_CanvasLevel.FadeOut(inGameFlowParameters.gameOverFadeOutDuration);
        RestartLevel();
        yield return m_CanvasLevel.FadeIn(inGameFlowParameters.fadeInDuration);
    }
}
