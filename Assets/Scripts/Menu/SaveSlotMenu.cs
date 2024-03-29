using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using CommonsHelper;
using CommonsPattern;

public class SaveSlotMenu : Menu
{
    [Header("Assets")]
    
    [Tooltip("Save Slot Parameters Data")]
    public SaveSlotParameters saveSlotParameters;
    
    [Tooltip("Save Slot Container Button Prefab")]
    public GameObject saveSlotContainerButtonPrefab;
    
    
    [Header("Child references")]
    
    [Tooltip("Header text")]
    public TextMeshProUGUI headerText;

    [Tooltip("No Save slot button")]
    public Button noSaveSlotButton;
    
    [Tooltip("Save Slots parent")]
    public Transform saveSlotsParent;
    
    [Tooltip("Back button")]
    public Button buttonBack;
    
    
    /* Cached references */
    
    /// Array of save slot container widgets
    private SaveSlotContainerWidget[] m_SaveSlotContainerWidgets;
    
    
    /* State */

    /// Saved Play Mode the Save Slot Menu was entered for (Story or Arcade)
    private SavedPlayMode m_SavedPlayMode;
    
    /// Setter for m_SavedPlayMode. Show is an override and we cannot change the signature to take a SavedPlayMode,
    /// so this allows us to still set the SavedPlayMode from another class.
    public SavedPlayMode SavedPlayMode
    {
        set { m_SavedPlayMode = value; }
    }

    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(saveSlotParameters != null, this, "[SaveSlotMenu] Awake: Save Slot Parameters not set on {0}", this);
        Debug.AssertFormat(saveSlotContainerButtonPrefab != null, this, "[SaveSlotMenu] Awake: Save Slot Container Button Prefab not set on {0}", this);
        Debug.AssertFormat(headerText != null, this, "[SaveSlotMenu] Awake: Header Text not set on {0}", this);
        Debug.AssertFormat(noSaveSlotButton != null, this, "[SaveSlotMenu] Awake: No Save Slot Button not set on {0}", this);
        Debug.AssertFormat(saveSlotsParent.IsChildOf(transform), this, "[SaveSlotMenu] Awake: Save Slots Parent is not a child (in the broad sense) of {0}", this);
        Debug.AssertFormat(saveSlotsParent != null, this, "[SaveSlotMenu] Awake: Save Slots Parent not set on {0}", this);
        Debug.AssertFormat(buttonBack != null, this, "[SaveSlotMenu] Awake: Button Back not set on {0}", this);
        #endif
        
        m_SaveSlotContainerWidgets = new SaveSlotContainerWidget[saveSlotParameters.saveSlotsCount];

        // No Save slot button just starts the first level without setting up any save, for a simple run 
        noSaveSlotButton.onClick.AddListener(StartGameWithoutSave);
        
        buttonBack.onClick.AddListener(GoBack);
    }

    private void OnDestroy()
    {
        if (noSaveSlotButton)
        {
            noSaveSlotButton.onClick.RemoveAllListeners();
        }
        
        if (buttonBack)
        {
            buttonBack.onClick.RemoveAllListeners();
        }
        
        // don't bother removing listeners from dynamic buttons, as we'd need to check if each of them exists
        // RemoveAllListeners has always been a bonus anyway, since the buttons have always been children of the
        // object with the script adding the listeners, and therefore they'd be destroyed together with the parent object
        // of course, make sure that saveSlotsParent is really on a child / grandchild / itself (done in Awake's Assert)
    }

    public override void Show()
    {
        LevelDataList levelDataList = MainMenuManager.Instance.levelDataList;

        gameObject.SetActive(true);
        
        noSaveSlotButton.Select();
        
        // Display header corresponding to Saved Play Mode
        headerText.text = (m_SavedPlayMode == SavedPlayMode.Story ? "Story mode" : "Arcade mode");
        
        UIPoolHelper.LazyInstantiateWidgets(saveSlotContainerButtonPrefab, saveSlotParameters.saveSlotsCount, saveSlotsParent);

        for (int i = 0; i < saveSlotParameters.saveSlotsCount; i++)
        {
            Transform saveSlotTransform = saveSlotsParent.GetChild(i);
            
            // Initialise widget model and view
            m_SaveSlotContainerWidgets[i] = saveSlotTransform.GetComponentOrFail<SaveSlotContainerWidget>();

            switch (m_SavedPlayMode)
            {
                case SavedPlayMode.Story:
                    PlayerSaveStory? optionalPlayerSaveStory = SessionManager.ReadJsonFromSaveFile<PlayerSaveStory>(m_SavedPlayMode, i);
                    if (optionalPlayerSaveStory.HasValue)
                    {
                        PlayerSaveStory playerSaveStory = optionalPlayerSaveStory.Value;
                        
                        // Player finished game on this slot iff the next level is last level index + 1
                        bool isComplete = playerSaveStory.nextLevelIndex >= levelDataList.levelDataArray.Length;
                        m_SaveSlotContainerWidgets[i].Init(m_SavedPlayMode, i);
                        m_SaveSlotContainerWidgets[i].InitFilled(isComplete, playerSaveStory.nextLevelIndex);
                    }
                    else
                    {
                        m_SaveSlotContainerWidgets[i].Init(m_SavedPlayMode, i);
                        m_SaveSlotContainerWidgets[i].InitEmpty();
                    }
                    break;
                case SavedPlayMode.Arcade:
                    PlayerSaveArcade? optionalPlayerSaveArcade = SessionManager.ReadJsonFromSaveFile<PlayerSaveArcade>(m_SavedPlayMode, i);
                    if (optionalPlayerSaveArcade.HasValue)
                    {
                        PlayerSaveArcade playerSaveArcade = optionalPlayerSaveArcade.Value;
                        bool isComplete = playerSaveArcade.nextLevelIndex >= levelDataList.levelDataArray.Length;
                        m_SaveSlotContainerWidgets[i].Init(m_SavedPlayMode, i);
                        m_SaveSlotContainerWidgets[i].InitFilled(isComplete, playerSaveArcade.nextLevelIndex);
                    }
                    else
                    {
                        m_SaveSlotContainerWidgets[i].Init(m_SavedPlayMode, i);
                        m_SaveSlotContainerWidgets[i].InitEmpty();
                    }
                    break;
            }
        }
    }

    public override void Hide()
    {
        EventSystem.current.SetSelectedGameObject(null);

        gameObject.SetActive(false);
    }

    public override bool ShouldShowTitle()
    {
        return false;
    }
    
    public override bool CanGoBack()
    {
        return true;
    }
        
    private void StartGameWithoutSave()
    {
        // Still notify Session Manager that we enter story/arcade mode so it is in the correct mode,
        // but pass slot index: -1 since there is no save associated to the session
        switch (m_SavedPlayMode)
        {
            case SavedPlayMode.Story:
                SessionManager.Instance.EnterStoryMode(-1, 0);
                break;
            case SavedPlayMode.Arcade:
                SessionManager.Instance.EnterArcadeMode(-1, 0);
                break;
        }
        
        // Always start from first level, since we're playing without save
        MainMenuManager.Instance.StartLevel(0);
    }
    
    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
