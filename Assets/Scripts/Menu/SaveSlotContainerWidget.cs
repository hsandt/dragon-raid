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
    
    /// Next level to load when resuming that save
    private int m_NextLevelIndex;
    
    
    public void InitEmpty()
    {
        m_IsFilled = false;
        
        // Empty save always create a new game that starts on first level
        m_NextLevelIndex = 0;

        RefreshVisual();
    }

    public void InitFilled(int nextLevelIndex)
    {
        m_IsFilled = true;
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
        MainMenuManager.Instance.StartLevel(m_NextLevelIndex);
    }
}
