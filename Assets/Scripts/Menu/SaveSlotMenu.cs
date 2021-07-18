using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityConstants;
using CommonsPattern;

public class SaveSlotMenu : Menu
{
    [Header("Assets")]
    
    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;
    
    [Tooltip("Save Slot Container Button Prefab")]
    public GameObject saveSlotContainerButtonPrefab;
    
    
    [Header("Child references")]
    
    [Tooltip("Save Slots parent")]
    public Transform saveSlotsParent;
    
    [Tooltip("Back button")]
    public Button buttonBack;


    private void Awake()
    {
        buttonBack.onClick.AddListener(GoBack);

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(levelDataList != null, this, "[MainMenu] Awake: Level Data List not set on {0}", this);
        #endif
    }

    private void OnDestroy()
    {
        if (buttonBack)
        {
            buttonBack.onClick.RemoveListener(GoBack);
        }
    }
    
    public override void Show()
    {
        gameObject.SetActive(true);
        
        UIPoolHelper.LazyInstantiateWidgets(saveSlotContainerButtonPrefab, 3, saveSlotsParent);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override bool ShouldShowTitle()
    {
        return false;
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

    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
