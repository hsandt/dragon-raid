using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveSlotContainerWidget : MonoBehaviour
{
    [Header("Child references")]
    
    [Tooltip("Next level text")]
    public TextMeshProUGUI nextLevelText;
    
    
    /* State */
    
    /// True if the slot contains a save, false if empty
    private bool m_IsFilled;
    
    /// Which Saved Play Mode does this widget represent? (Story or Arcade)
    private SavedPlayMode m_SavedPlayMode;
    
    /// Next level to load when resuming that save
    private int m_NextLevelIndex;
    
    
    public void InitEmpty(SavedPlayMode savedPlayMode)
    {
        m_IsFilled = false;
        m_SavedPlayMode = savedPlayMode;
        
        // Empty save always create a new game that starts on first level
        m_NextLevelIndex = 0;

        RefreshVisual();
    }

    public void InitFilled(int nextLevelIndex, SavedPlayMode savedPlayMode)
    {
        m_IsFilled = true;
        m_SavedPlayMode = savedPlayMode;
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
        if (m_IsFilled)
        {
            // TODO: load existing save
            switch (m_SavedPlayMode)
            {
                case SavedPlayMode.Story:
                    break;
                case SavedPlayMode.Arcade:
                    break;
            }
        }
        else
        {
            switch (m_SavedPlayMode)
            {
                case SavedPlayMode.Story:
                    SessionManager.Instance.StartNewSaveStory();
                    break;
                case SavedPlayMode.Arcade:
                    SessionManager.Instance.StartNewSaveArcade();
                    break;
            }
        }
        
        // If slot is filled, next level index is 0, so this statement always works
        MainMenuManager.Instance.StartLevel(m_NextLevelIndex);
    }
}
