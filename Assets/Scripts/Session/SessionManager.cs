using System;
using System.IO;
using UnityEngine;

using CommonsPattern;

public class SessionManager : SingletonManager<SessionManager>
{
    /* State: Save metadata */

    /// Current play mode (None in title menu)
    private PlayMode m_CurrentPlayMode;
    
    /// Index of save slot for this saved play mode
    private int m_SaveSlotIndex;
    
    /* State: Save data */
    
    /// Index of the level to load when the player loads this Save
    /// As soon as the player finishes a level, this is incremented in the current save
    public int m_NextLevelIndex;
    

    protected override void Init()
    {
        base.Init();

        m_CurrentPlayMode = PlayMode.None;
        m_NextLevelIndex = -1;
    }

    public void EnterStoryMode(int saveSlotIndex, int enteredLevelIndex)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(m_CurrentPlayMode == PlayMode.None,
            "[SessionManager] EnterStoryMode: m_CurrentPlayMode is not PlayMode.None, it will be set to " +
            "PlayMode.Story but this is not supposed to happen.");
        #endif
        m_CurrentPlayMode = PlayMode.Story;
        m_SaveSlotIndex = saveSlotIndex;
        m_NextLevelIndex = enteredLevelIndex;
    }

    public void EnterArcadeMode(int saveSlotIndex, int enteredLevelIndex)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(m_CurrentPlayMode == PlayMode.None,
            "[SessionManager] EnterStoryMode: m_CurrentPlayMode is not PlayMode.None, it will be set to " +
            "PlayMode.Arcade but this is not supposed to happen.");
        #endif
        m_CurrentPlayMode = PlayMode.Arcade;
        m_SaveSlotIndex = saveSlotIndex;
        m_NextLevelIndex = enteredLevelIndex;
    }
    
    public void ExitCurrentPlayMode()
    {
        m_CurrentPlayMode = PlayMode.None;
    }

    public void NotifyLevelFinished(int finishedLevelIndex)
    {
        // Only set next level index in story or arcade mode (level select mode just goes back to level select menu)
        if (m_CurrentPlayMode == PlayMode.Story || m_CurrentPlayMode == PlayMode.Arcade)
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
        Debug.AssertFormat(m_SaveSlotIndex >= 0, "[SessionManager] SaveCurrentProgress: save slot index should be positive, got {0}", m_SaveSlotIndex);
        #endif

        // Only set next level index in story or arcade mode (level select mode just goes back to level select menu)
        if (m_CurrentPlayMode == PlayMode.Story)
        {
            PlayerSaveStory playerSaveStory = new PlayerSaveStory
            {
                nextLevelIndex = m_NextLevelIndex
            };

            WriteJsonToSaveFile(SavedPlayMode.Story, m_SaveSlotIndex, playerSaveStory);
        }
        else if (m_CurrentPlayMode == PlayMode.Arcade)
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
            Debug.LogFormat("[SessionManager] WriteJsonToSaveFile: could not create text file '{0}', " +
                "due to exception:\n{1}", saveFilePath, e);
            return;
        }
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogFormat("[SessionManager] Saved progress in saveFilePath: {0} with Json: {1}", saveFilePath,
            playerSaveStoryJson);
        #endif
    }

    private static string GetSaveFilePath(SavedPlayMode savedPlayMode, int saveSlotIndex)
    {
        // Ex: /home/USERNAME/.config/unity3d/komehara/Dragon Raid/saves/Arcade_01.save
        return Path.Combine(Application.persistentDataPath, "saves", $"{savedPlayMode}_{saveSlotIndex:00}.save");
    }
}
