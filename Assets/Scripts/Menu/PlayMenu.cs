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
            buttonStory.onClick.RemoveListener(StartGame);
        }
        if (buttonArcade)
        {
            buttonArcade.onClick.RemoveListener(StartArcade);
        }
        if (buttonLevelSelect)
        {
            buttonLevelSelect.onClick.RemoveListener(EnterLevelSelect);
        }
        if (buttonBack)
        {
            buttonBack.onClick.RemoveListener(GoBack);
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

    private void StartStory()
    {
        StartGame();
    }

    private void StartArcade()
    {
        StartGame();
    }
        
    private void StartGame()
    {
        if (levelDataList.levelDataArray.Length > 0)
        {
            LevelData levelData = levelDataList.levelDataArray[0];
            if (levelData != null)
            {
                SceneManager.LoadScene((int)levelData.sceneEnum);
            }
            else
            {
                Debug.LogErrorFormat(levelDataList, "[MainMenu] StartGame: Level Data List first entry is null");
            }
        }
        else
        {
            Debug.LogErrorFormat(levelDataList, "[MainMenu] StartGame: Level Data List is empty");
        }
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
