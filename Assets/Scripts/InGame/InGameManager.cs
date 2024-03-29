using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityConstants;
using CommonsDebug;
using CommonsHelper;
using CommonsPattern;

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

    /// Cached Main Camera custom component reference
    private InGameCamera m_InGameCamera;

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

    /// True iff game is paused / pause menu is open
    private bool m_IsGamePaused;

    /// True iff level finish sequence is playing
    private bool m_IsFinishingLevel;
    public bool IsFinishingLevel => m_IsFinishingLevel;

    /// True iff game over -> restart sequence (game over or via menu) is playing
    private bool m_IsPlayingRestartSequence;
    public bool IsPlayingRestartSequence => m_IsPlayingRestartSequence;

    public bool CanPauseGame => !m_IsGamePaused && !m_IsFinishingLevel && !m_IsPlayingRestartSequence;
    // We let the game run during the menu restart sequence (and it's already running when the game over restart
    // sequence starts), so we are checking that m_IsGamePaused is false on purpose.
    public bool CanPlayRestartLevelSequence => !m_IsGamePaused && !m_IsFinishingLevel && !m_IsPlayingRestartSequence;
    // unlike CanPlayRestartLevelSequence, CanRestartLevel checks for the actual Restart which, outside Cheat Restart,
    // happen right in the middle of the Restart Sequence, so we should not check for !m_IsPlayingRestartSequence
    public bool CanRestartLevel => !m_IsGamePaused && !m_IsFinishingLevel;
    public bool CanFinishLevel => !m_IsGamePaused && !m_IsFinishingLevel && !m_IsPlayingRestartSequence;
    public bool CanAnyEntityBeDamagedOrHealed => !m_IsGamePaused && !m_IsFinishingLevel && !m_IsPlayingRestartSequence;
    public bool CanTriggerSpatialProgressEvent => !m_IsGamePaused && !m_IsFinishingLevel && !m_IsPlayingRestartSequence;

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    public bool CanUseCheat => !m_IsGamePaused && !m_IsFinishingLevel && !m_IsPlayingRestartSequence;
    #endif

    protected override void Init()
    {
        base.Init();

        Camera mainCameraNativeScript = Camera.main;
        DebugUtil.Assert(mainCameraNativeScript != null, "[InGameManager] No Main Camera found in scene");
        m_InGameCamera = mainCameraNativeScript.GetComponentOrFail<InGameCamera>();

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

        ScrollingManager.Instance.Setup();
        SpatialEventManager.Instance.Setup();
        EnemyWaveManager.Instance.Setup();
        HUD.Instance.Setup();

        if (m_LevelData != null)
        {
            MusicManager.Instance.PlayBgm(m_LevelData.bgm);
        }

        InitialSpawnPlayerCharacter();

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Apply Cheat level setup after spawning player character,
        // it will move it relatively to its start position if needed
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

            AssignPlayerCharacterController();
        }
    }

    private void RespawnPlayerCharacter()
    {
        // Respawn Player Character
        // Unlike InitialSpawnPlayerCharacter above, we don't need to assign HUD view nor input controller,
        // and we don't init extra lives
        if (m_PlayerSpawnTransform != null)
        {
            // Respawn character at determined position relative to camera
            // (we could also respawn at the last position before death depending on design)
            m_PlayerCharacterMaster = PlayerCharacterPoolManager.Instance.SpawnCharacter(m_InGameCamera.playerRespawnPosition.position);

            // Start respawn invincibility (with visual feedback)
            var playerCharacterHealthSystem = m_PlayerCharacterMaster.GetComponentOrFail<HealthSystem>();
            playerCharacterHealthSystem.StartRespawnInvincibility();
        }
    }

    private void AssignPlayerCharacterController()
    {
        var playerCharacterController = m_PlayerCharacterMaster.GetComponentOrFail<PlayerCharacterController>();
        InGameInputManager.Instance.SetPlayerCharacterController(playerCharacterController);
    }

    private void ClearLevel()
    {
        // First clean up references to avoid referencing cleared objects
        // Do not clear cached scene references m_PlayerSpawnTransform and m_LevelData yet,
        // those can only be destroyed when loading a new scene (assuming InGameManager is flagged DontDestroyOnLoad
        // so it preserves the destroyed reference), so we'll handle this in FinishLevel
        m_PlayerCharacterMaster = null;
        InGameInputManager.Instance.SetPlayerCharacterController(null);

        // Setup does most of the job, so this is just to reset layer positions
        ScrollingManager.Instance.Clear();

        // Clear all waves (this only clears wave information, we need to despawn all enemies below so things stay synced)
        EnemyWaveManager.Instance.Clear();

        // Despawn the player character
        // We use the generic Pool API to release all objects, but it really only releases 1 here
        PlayerCharacterPoolManager.Instance.ReleaseAllObjects();

        // Despawn all enemies
        // (this is a low-level method and will not trigger any death / cleared wave event)
        EnemyPoolManager.Instance.ReleaseAllObjects();

        // Clean up all projectiles
        ProjectilePoolManager.Instance.ReleaseAllObjects();

        // Clean up all PickUp
        PickUpPoolManager.Instance.ReleaseAllObjects();

        // Clean up all FX
        FXPoolManager.Instance.ReleaseAllObjects();

        // Clean up all environment props
        foreach (Transform environmentPropTransform in ScrollingManager.Instance.GetMidgroundLayer())
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
        // Don't allow level restart during finish sequence to avoid invalid states like loading next level async
        // but restarting this level in the middle
        if (CanRestartLevel)
        {
            ClearLevel();
            SetupLevel();
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat("[InGameManager] RestartLevel: CanRestartLevel is false " +
                                 "(m_IsGamePaused: {0}, m_IsFinishingLevel: {1})",
                m_IsGamePaused, m_IsFinishingLevel);
        }
        #endif
    }

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
            PickUpPoolManager.Instance.PauseAllPickUp();
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
        PickUpPoolManager.Instance.ResumeAllPickUp();
        FXPoolManager.Instance.ResumeAllFX();
    }

    public bool TryFinishLevel()
    {
        if (!CanFinishLevel)
        {
            return false;
        }

        FinishLevel();
        return true;
    }

    public void FinishLevel()
    {
        if (CanFinishLevel)
        {
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
            Debug.LogErrorFormat("[InGameManager] FinishLevel: CanFinishLevel is false " +
                                 "(m_IsGamePaused: {0}, m_IsFinishingLevel: {1})", m_IsGamePaused, m_IsFinishingLevel);
        }
        #endif
    }

    private IEnumerator FinishLevelAsync()
    {
        m_IsFinishingLevel = true;

        yield return new WaitForSeconds(inGameFlowParameters.performanceAssessmentDelay);

        // Show performance assessment canvas
        PerformanceAssessment.Instance.Show();

        yield return new WaitForSeconds(inGameFlowParameters.loadNextLevelDelay);

        LoadNextLevelOrGoBackToTitle();

        m_IsFinishingLevel = false;
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

    public void PlayGameOverRestartSequence()
    {
        if (CanPlayRestartLevelSequence)
        {
            StartCoroutine(PlayGameOverRestartSequenceAsync());
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat("[InGameManager] PlayGameOverRestartSequence: CanPlayRestartLevelSequence is false " +
                                 "(m_IsGamePaused: {0}, m_IsFinishingLevel: {1}, m_IsPlayingRestartSequence: {2})",
                m_IsGamePaused, m_IsFinishingLevel, m_IsPlayingRestartSequence);
        }
        #endif
    }

    private IEnumerator PlayGameOverRestartSequenceAsync()
    {
        m_IsPlayingRestartSequence = true;

        yield return new WaitForSeconds(inGameFlowParameters.gameOverDelay);
        yield return m_CanvasLevel.FadeOut(inGameFlowParameters.gameOverFadeOutDuration);
        RestartLevel();
        yield return m_CanvasLevel.FadeIn(inGameFlowParameters.fadeInDuration);

        m_IsPlayingRestartSequence = false;
    }

    public void PlayMenuRestartSequence()
    {
        if (CanPlayRestartLevelSequence)
        {
            StartCoroutine(PlayMenuRestartSequenceAsync());
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat("[InGameManager] PlayMenuRestartSequence: CanPlayRestartLevelSequence is false " +
                                 "(m_IsGamePaused: {0}, m_IsFinishingLevel: {1}, m_IsPlayingRestartSequence: {2})",
                m_IsGamePaused, m_IsFinishingLevel, m_IsPlayingRestartSequence);
        }
        #endif
    }

    private IEnumerator PlayMenuRestartSequenceAsync()
    {
        m_IsPlayingRestartSequence = true;

        yield return m_CanvasLevel.FadeOut(inGameFlowParameters.menuRestartFadeOutDuration);
        RestartLevel();
        yield return m_CanvasLevel.FadeIn(inGameFlowParameters.fadeInDuration);

        m_IsPlayingRestartSequence = false;
    }
}
