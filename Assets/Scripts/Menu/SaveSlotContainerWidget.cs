using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotContainerWidget : MonoBehaviour
{
    [Header("Child references")]
    
    [Tooltip("Save slot text")]
    public TextMeshProUGUI saveSlotText;
    
    [Tooltip("Save slot button")]
    public Button saveSlotButton;
    
    [Tooltip("Delete save button")]
    public Button deleteSaveButton;
    
    
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
    
    // TODO: put the whole deserialized player save object here
    /// Next level to load when resuming that save
    private int m_NextLevelIndex;


    private void Awake()
    {
        // Only register button callback once, in Awake rather than the Init methods,
        // to avoid multiple registration. In counterpart, we'll check for button validity/behavior
        // inside the callbacks
        saveSlotButton.onClick.AddListener(OnConfirmSaveSlotSelection);
        deleteSaveButton.onClick.AddListener(OnDeleteSave);
    }
    
    private void OnDestroy()
    {
        if (saveSlotButton)
        {
            saveSlotButton.onClick.RemoveAllListeners();
        }

        if (deleteSaveButton)
        {
            deleteSaveButton.onClick.RemoveAllListeners();
        }
    }

    public void Init(SavedPlayMode savedPlayMode, int slotIndex)
    {
        // Initialise parameters identifying the slot
        m_SavedPlayMode = savedPlayMode;
        m_SlotIndex = slotIndex;
    }

    public void InitEmpty()
    {
        // Initialise content parameters for an empty slot
        // Must be called after Init
        
        m_IsFilled = false;
        m_IsComplete = false;
        
        // Empty save always create a new game that starts on first level
        m_NextLevelIndex = 0;

        // Empty slot cannot delete save, so hide delete save button
        deleteSaveButton.gameObject.SetActive(false);

        RefreshVisual();
    }

    public void InitFilled(bool isComplete, int nextLevelIndex)
    {
        // Initialise content parameters for a filled slot
        // Must be called after Init

        m_IsFilled = true;
        m_IsComplete = isComplete;
        m_NextLevelIndex = nextLevelIndex;
        
        // Show delete save button in case it was hidden due to previous InitEmpty call + UI pool object reuse
        deleteSaveButton.gameObject.SetActive(true);

        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (m_IsFilled)
        {
            saveSlotText.text = $"Level {m_NextLevelIndex}";
        }
        else
        {
            saveSlotText.text = "Empty";
        }
    }
    
    private void OnConfirmSaveSlotSelection()
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
    
    private void OnDeleteSave()
    {
        if (m_IsFilled)
        {
            // No confirm prompt for now, so immediately delete save on this slot
            SessionManager.DeleteSaveFile(m_SavedPlayMode, m_SlotIndex);
            
            // Immediately update widget parameters and visual to show empty slot
            InitEmpty();
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat(this, "[SaveSlotContainerWidget] OnDeleteSave on {0}: slot is not filled, Delete button shouldn't even be visible.", this);
        }
        #endif
    }
}
