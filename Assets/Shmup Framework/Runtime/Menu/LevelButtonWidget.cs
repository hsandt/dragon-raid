using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonWidget : MonoBehaviour
{
    [Header("Child references")]
    
    [Tooltip("Level text")]
    public TextMeshProUGUI levelText;
    
    
    /* Sibling components */
    
    private Button m_LevelButton;
    
    
    /* Main parameters */
    
    /// Index of level represented by this widget, and that is entered on confirm
    private int m_LevelIndex;
    

    private void Awake()
    {
        m_LevelButton = this.GetComponentOrFail<Button>();
        
        // Only register button callback once, in Awake rather than the Init methods,
        // to avoid multiple registration.
        m_LevelButton.onClick.AddListener(OnConfirmLevelSelection);
    }
    
    private void OnDestroy()
    {
        if (m_LevelButton)
        {
            m_LevelButton.onClick.RemoveAllListeners();
        }
    }

    public void Init(int levelIndex)
    {
        m_LevelIndex = levelIndex;
        
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        levelText.text = $"Level {m_LevelIndex}";
    }

    public void Select()
    {
        m_LevelButton.Select();
    }
    
    private void OnConfirmLevelSelection()
    {
        SessionManager.Instance.EnterLevelSelectMode();
        
        // Start selected level
        MainMenuManager.Instance.StartLevel(m_LevelIndex);
    }
}
