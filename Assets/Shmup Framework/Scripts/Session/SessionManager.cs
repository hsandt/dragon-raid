using System;
using System.IO;
using UnityEngine;

using CommonsPattern;

public class SessionManager : SingletonManager<SessionManager>
{
    /* State: Save metadata */

    /// Current play mode (None in title menu)
    private SessionPlayMode m_CurrentPlayMode;

    /// Index of save slot for this saved play mode
    private int m_SaveSlotIndex;

    /* State: Save data */

    /// Index of the level to load when the player loads this Save
    /// As soon as the player finishes a level, this is incremented in the current save
    private int m_NextLevelIndex;


    protected override void Init()
    {
        base.Init();

        m_CurrentPlayMode = SessionPlayMode.None;
        m_NextLevelIndex = -1;
    }

    /// If current mode is None, fallback to Arcade mode with No Save (slot index: -1)
    /// and with indicated entered level index (obtained from current scene).
    /// This is only useful when testing in the editor and playing directly from a Level scene,
    /// so that EnterStoryMode/EnterArcadeMode was never called (it's normally called from the Title menu)
    public void EnterFallbackModeIfNone(int enteredLevelIndex)
    {
        if (m_CurrentPlayMode == SessionPlayMode.None)
        {
            EnterArcadeMode(-1, enteredLevelIndex);
        }
    }

    public void EnterStoryMode(int saveSlotIndex, int enteredLevelIndex)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_CurrentPlayMode == SessionPlayMode.None,
            "[SessionManager] EnterStoryMode: m_CurrentPlayMode is {0}, expected PlayMode.None. It will be set to " +
            "PlayMode.Story anyway, but this is not supposed to happen.", m_CurrentPlayMode);
        #endif
        m_CurrentPlayMode = SessionPlayMode.Story;
        m_SaveSlotIndex = saveSlotIndex;
        m_NextLevelIndex = enteredLevelIndex;
    }

    public void EnterArcadeMode(int saveSlotIndex, int enteredLevelIndex)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_CurrentPlayMode == SessionPlayMode.None,
            "[SessionManager] EnterStoryMode: m_CurrentPlayMode is {0}, expected PlayMode.None. It will be set to " +
            "PlayMode.Arcade anyway, but this is not supposed to happen.", m_CurrentPlayMode);
        #endif
        m_CurrentPlayMode = SessionPlayMode.Arcade;
        m_SaveSlotIndex = saveSlotIndex;
        m_NextLevelIndex = enteredLevelIndex;
    }

    public void EnterLevelSelectMode()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_CurrentPlayMode == SessionPlayMode.None,
            "[SessionManager] EnterLevelSelectMode: m_CurrentPlayMode is {0}, expected PlayMode.None. It will be set to " +
            "PlayMode.LevelSelect anyway, but this is not supposed to happen.", m_CurrentPlayMode);
        #endif
        m_CurrentPlayMode = SessionPlayMode.LevelSelect;
    }

    public void ExitCurrentPlayMode()
    {
        m_CurrentPlayMode = SessionPlayMode.None;
    }

    public void NotifyLevelFinished(int finishedLevelIndex)
    {
        // Only set next level index in story or arcade mode (level select mode just goes back to level select menu)
        if (m_CurrentPlayMode == SessionPlayMode.Story || m_CurrentPlayMode == SessionPlayMode.Arcade)
        {
            // Set next level index (alternatively, increment it, assuming it was correct)
            // If we finished the last level, this index will be just after the last once, meaning "finished game"
            m_NextLevelIndex = finishedLevelIndex + 1;

            // Auto-save unless using No Save slot (index: -1)
            if (m_SaveSlotIndex >= 0)
            {
                SaveCurrentProgress();
            }
        }
    }

    public void SaveCurrentProgress()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_SaveSlotIndex >= 0, "[SessionManager] SaveCurrentProgress: save slot index should be >= 0, got {0}", m_SaveSlotIndex);
        #endif

        // Only set next level index in story or arcade mode (level select mode just goes back to level select menu)
        if (m_CurrentPlayMode == SessionPlayMode.Story)
        {
            PlayerSaveStory playerSaveStory = new PlayerSaveStory
            {
                nextLevelIndex = m_NextLevelIndex
            };

            WriteJsonToSaveFile(SavedPlayMode.Story, m_SaveSlotIndex, playerSaveStory);
        }
        else if (m_CurrentPlayMode == SessionPlayMode.Arcade)
        {
            PlayerSaveArcade playerSaveArcade = new PlayerSaveArcade
            {
                nextLevelIndex = m_NextLevelIndex
            };

            WriteJsonToSaveFile(SavedPlayMode.Arcade, m_SaveSlotIndex, playerSaveArcade);
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat(this, "[SessionManager] SaveCurrentProgress: m_CurrentPlayMode is " +
               "not PlayMode.Story nor PlayMode.Arcade, we shouldn't try to save current progress.");
        }
        #endif
    }

    // T should be PlayerSaveStory or PlayerSaveArcade
    public static T? ReadJsonFromSaveFile<T>(SavedPlayMode savedPlayMode, int saveSlotIndex) where T : struct
    {
        string saveFilePath = GetSaveFilePath(savedPlayMode, saveSlotIndex);
        if (File.Exists(saveFilePath))
        {
            string playerSaveJson = File.ReadAllText(saveFilePath);
            T playerSave = JsonUtility.FromJson<T>(playerSaveJson);
            return playerSave;
        }

        return null;
    }

    // T should be PlayerSaveStory or PlayerSaveArcade
    private static void WriteJsonToSaveFile<T>(SavedPlayMode savedPlayMode, int saveSlotIndex, T playerSave)
    {
        string playerSaveStoryJson = JsonUtility.ToJson(playerSave);
        string saveFilePath = GetSaveFilePath(savedPlayMode, saveSlotIndex);

        string saveFileDirectoryPath = Path.GetDirectoryName(saveFilePath);
        if (saveFileDirectoryPath != null && !Directory.Exists(saveFileDirectoryPath))
        {
            try
            {
                Directory.CreateDirectory(saveFileDirectoryPath);
            }
            catch (Exception e)
            {
                Debug.LogFormat("[SessionManager] WriteJsonToSaveFile: could not create directory '{0}', " +
                    "so cannot write to file '{1}', due to exception:\n{2}", saveFileDirectoryPath, saveFilePath, e);
                return;
            }
        }

        try
        {
            // Write new file, or overwrite existing file at this path
            using (StreamWriter streamWriter = File.CreateText(saveFilePath))
            {
                streamWriter.Write(playerSaveStoryJson);
            }
        }
        catch (Exception e)
        {
            Debug.LogFormat("[SessionManager] WriteJsonToSaveFile: could not create text file '{0}' " +
                "due to exception:\n{1}", saveFilePath, e);
            return;
        }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogFormat("[SessionManager] Saved progress in saveFilePath: {0} with Json: {1}", saveFilePath,
            playerSaveStoryJson);
        #endif
    }

    // T should be PlayerSaveStory or PlayerSaveArcade
    public static void DeleteSaveFile(SavedPlayMode savedPlayMode, int saveSlotIndex)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(saveSlotIndex >= 0, "[SessionManager] DeleteSaveFile: save slot index should be >= 0, got {0}", saveSlotIndex);
        #endif

        string saveFilePath = GetSaveFilePath(savedPlayMode, saveSlotIndex);

        if (File.Exists(saveFilePath))
        {
            try
            {
                File.Delete(saveFilePath);
            }
            catch (Exception e)
            {
                Debug.LogFormat("[SessionManager] DeleteSaveFile: could not delete text file '{0}' " +
                                "due to exception:\n{1}", saveFilePath, e);
                return;
            }
        }
        else
        {
            Debug.LogFormat("[SessionManager] DeleteSaveFile: could not delete text file '{0}' " +
                            "because it doesn't exist.", saveFilePath);
            return;
        }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogFormat("[SessionManager] Deleted save file: {0}", saveFilePath);
        #endif
    }

    private static string GetSaveFilePath(SavedPlayMode savedPlayMode, int saveSlotIndex)
    {
        // Ex: /home/USERNAME/.config/unity3d/My Company/My Shmup/saves/Arcade_01.save
        return Path.Combine(Application.persistentDataPath, "saves", $"{savedPlayMode}_{saveSlotIndex:00}.save");
    }
}
