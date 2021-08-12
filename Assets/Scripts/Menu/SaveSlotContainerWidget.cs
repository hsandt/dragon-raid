using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveSlotContainerWidget : MonoBehaviour
{
    [Header("Child references")]
    
    [Tooltip("Next level text")]
    public TextMeshProUGUI nextLevelText;
    
    
    /* Main parameters */
    
    /// Which Saved Play Mode does this widget represent? (Story or Arcade)
    private SavedPlayMode m_SavedPlayMode;
    
    /// Index of save slot for this saved play mode
    private int m_SlotIndex;

    
    /* Derived parameters */

    /// True if the slot contains a save, false if empty
    private bool m_IsFilled;
    
    /// True if the player has finished the last level in this save slot
    /// If so, m_NextLevelIndex must be last level index + 1 so it's important
    /// to check this flag first to avoid loading an invalid level
    private bool m_IsComplete;
    
    /// Next level to load when resuming that save
    private int m_NextLevelIndex;
    
    
    public void InitEmpty(SavedPlayMode savedPlayMode, int slotIndex)
    {
        m_SavedPlayMode = savedPlayMode;
        m_SlotIndex = slotIndex;
        
        m_IsFilled = false;
        m_IsComplete = false;
        
        // Empty save always create a new game that starts on first level
        m_NextLevelIndex = 0;

        RefreshVisual();
    }

    public void InitFilled(SavedPlayMode savedPlayMode, int slotIndex, bool isComplete, int nextLevelIndex)
    {
        m_SavedPlayMode = savedPlayMode;
        m_SlotIndex = slotIndex;

        m_IsFilled = true;
        m_IsComplete = isComplete;
        m_NextLevelIndex = nextLevelIndex;

        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (m_IsFilled)
        {
            nextLevelText.text = $"Level {m_NextLevelIndex}";
        }
        else
        {
            nextLevelText.text = "Empty";
        }
    }

    public void OnConfirm()
    {
        // If game has been completed, we should allow player to select any level to restart from
        // This hasn't been implemented yet though, so for now just restart the game from the last level
        // (if m_IsComplete, m_NextLevelIndex = last level index + 1 so just subtract 1)
        int nextLevelIndex = m_IsComplete ? m_NextLevelIndex - 1 : m_NextLevelIndex;
        
        switch (m_SavedPlayMode)
        {
            case SavedPlayMode.Story:
                SessionManager.Instance.EnterStoryMode(m_SlotIndex, nextLevelIndex);
                break;
            case SavedPlayMode.Arcade:
                SessionManager.Instance.EnterArcadeMode(m_SlotIndex, nextLevelIndex);
                break;
        }
        
        // Immediately save progress if starting from empty slot (new save)
        // This will effectively create a save for this slot, with next level index: 0
        if (!m_IsFilled)
        {
            SessionManager.Instance.SaveCurrentProgress();
        }
        
        // Start next level (0 if new save)
        MainMenuManager.Instance.StartLevel(nextLevelIndex);
    }
}
