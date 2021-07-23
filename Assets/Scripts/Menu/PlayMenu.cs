using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityConstants;

public class PlayMenu : Menu
{
    [Header("Assets")]
    
    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;
    
    
    [Header("Scene references")]
    
    [Tooltip("Story button")]
    public Button buttonStory;
    
    [Tooltip("Arcade button")]
    public Button buttonArcade;
    
    [Tooltip("Level Select button")]
    public Button buttonLevelSelect;
    
    [Tooltip("Back button")]
    public Button buttonBack;

    [Tooltip("Save Slot menu")]
    public SaveSlotMenu saveSlotMenu;
    
    [Tooltip("Level Select menu")]
    public LevelSelectMenu levelSelectMenu;


    private void Awake()
    {
        buttonStory.onClick.AddListener(StartStory);
        buttonArcade.onClick.AddListener(StartArcade);
        buttonLevelSelect.onClick.AddListener(EnterLevelSelect);
        buttonBack.onClick.AddListener(GoBack);

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(levelDataList != null, this, "[MainMenu] Awake: Level Data List not set on {0}", this);
        #endif
    }

    private void OnDestroy()
    {
        if (buttonStory)
        {
            buttonStory.onClick.RemoveAllListeners();
        }
        if (buttonArcade)
        {
            buttonArcade.onClick.RemoveAllListeners();
        }
        if (buttonLevelSelect)
        {
            buttonLevelSelect.onClick.RemoveAllListeners();
        }
        if (buttonBack)
        {
            buttonBack.onClick.RemoveAllListeners();
        }
    }
    
    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override bool ShouldShowTitle()
    {
        return true;
    }

    private void StartStory()
    {
        saveSlotMenu.SavedPlayMode = SavedPlayMode.Story;
        MainMenuManager.Instance.EnterMenu(saveSlotMenu);
    }

    private void StartArcade()
    {
        saveSlotMenu.SavedPlayMode = SavedPlayMode.Arcade;
        MainMenuManager.Instance.EnterMenu(saveSlotMenu);
    }

    private void EnterLevelSelect()
    {
        MainMenuManager.Instance.EnterMenu(levelSelectMenu);
    }
    
    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
