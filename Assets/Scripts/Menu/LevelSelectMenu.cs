using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityConstants;

public class LevelSelectMenu : Menu
{
    [Header("Assets")]
    
    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;
    
    
    [Header("Scene references")]
    
    [Tooltip("List of Start Level buttons")]
    public List<Button> buttonStartLevelList;

    [Tooltip("Back button")]
    public Button buttonBack;

    
    private void Awake()
    {
        buttonBack.onClick.AddListener(GoBack);

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(levelDataList != null, this, "[MainMenu] Awake: Level Data List not set on {0}", this);
        #endif
    }
    
    private void Start()
    {
        for (int i = 0; i < levelDataList.levelDataArray.Length; i++)
        {
            // eventually we'll do lazy pooling i.e. create any missing buttons (copying code from Anima) 
            // but for now we assume the buttons have been manually added
            if (i < buttonStartLevelList.Count && buttonStartLevelList[i] != null)
            {
                // quick trick to pass dynamic callback: copy variable to get constant in closure,
                // and pass lambda
                // eventually, we'll have a dedicated StartLevelButton with a member int levelIndex
                // and its own method StartLevel, so we won't need to pass the level index anymore
                int closureConstantLevelIndex = i;
                buttonStartLevelList[i].onClick.AddListener(() => StartLevel(closureConstantLevelIndex));
            }
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            else
            {
                Debug.LogErrorFormat(this, "[LevelSelectMenu] Start: buttonStartLevelList has no entry or null entry " +
                   "for index {0} yet levelDataList.levelDataArray.Length is {1}",
                    i, levelDataList.levelDataArray.Length);
            }
            #endif
        }
    }

    private void OnDestroy()
    {
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
        return false;
    }

    private void StartLevel(int levelIndex)
    {
        if (levelDataList.levelDataArray.Length > levelIndex)
        {
            LevelData levelData = levelDataList.levelDataArray[levelIndex];
            if (levelData != null)
            {
                SceneManager.LoadScene((int)levelData.sceneEnum);
            }
            else
            {
                Debug.LogErrorFormat(levelDataList, "[LevelSelectMenu] StartGame: Level Data List first entry is null");
            }
        }
        else
        {
            Debug.LogErrorFormat(levelDataList, "[LevelSelectMenu] StartGame: Level Data List is empty");
        }
    }
    
    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
