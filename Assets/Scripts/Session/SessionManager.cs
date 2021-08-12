using System.IO;
using UnityEngine;

using CommonsPattern;

public class SessionManager : SingletonManager<SessionManager>
{
    /* State: Save metadata */

    /// Current play mode (None in title menu)
    private PlayMode m_CurrentPlayMode;
    
    /// Index of save slot for this saved play mode
    private int m_SlotIndex;
    
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
        m_SlotIndex = saveSlotIndex;
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
        m_SlotIndex = saveSlotIndex;
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
            
            // Auto-save
            SaveCurrentProgress();
        }
    }

    private void SaveCurrentProgress()
    {
        // Only set next level index in story or arcade mode (level select mode just goes back to level select menu)
        if (m_CurrentPlayMode == PlayMode.Story)
        {
            PlayerSaveStory playerSaveStory = new PlayerSaveStory
            {
                nextLevelIndex = m_NextLevelIndex
            };

            WriteJsonToSaveFile(playerSaveStory);
        }
        else if (m_CurrentPlayMode == PlayMode.Arcade)
        {
            PlayerSaveArcade playerSaveArcade = new PlayerSaveArcade
            {
                nextLevelIndex = m_NextLevelIndex
            };
            
            WriteJsonToSaveFile(playerSaveArcade);
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat(this, "[SessionManager] SaveCurrentProgress: m_CurrentPlayMode is " +
               "not PlayMode.Story nor PlayMode.Arcade, we shouldn't try to save current progress.");
        }
        #endif
    }

    private void WriteJsonToSaveFile<T>(T playerSave)
    {
        string playerSaveStoryJson = JsonUtility.ToJson(playerSave);
        string saveFilePath = Path.Combine(Application.persistentDataPath, $"{m_CurrentPlayMode}_{m_SlotIndex:02}");
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogFormat("[SessionManager] Saved progress in saveFilePath: {0} with Json: {1}", saveFilePath,
            playerSaveStoryJson);
        #endif
        StreamWriter streamWriter = File.CreateText(saveFilePath);
        streamWriter.Write(playerSaveStoryJson);
        streamWriter.Close();
    }
}
