using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectMenu : Menu
{
    [Header("Child references")]
    
    [Tooltip("List of Start Level buttons")]
    public List<Button> buttonStartLevelList;

    [Tooltip("Back button")]
    public Button buttonBack;

    
    private void Awake()
    {
        buttonBack.onClick.AddListener(GoBack);
    }
    
    private void Start()
    {
        LevelDataList levelDataList = MainMenuManager.Instance.levelDataList;
        
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
                int closureLevelIndex = i;
                buttonStartLevelList[i].onClick.AddListener(() => MainMenuManager.Instance.StartLevel(closureLevelIndex));
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

        if (buttonStartLevelList.Count > 0)
        {
            buttonStartLevelList[0].Select();
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

    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
