using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityConstants;
using CommonsHelper;
using CommonsPattern;

public class SaveSlotMenu : Menu
{
    [Header("Assets")]
    
    [Tooltip("Save Slot Parameters Data")]
    public SaveSlotParameters saveSlotParameters;
    
    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;
    
    [Tooltip("Save Slot Container Button Prefab")]
    public GameObject saveSlotContainerButtonPrefab;
    
    
    [Header("Child references")]
    
    [Tooltip("Save Slots parent")]
    public Transform saveSlotsParent;
    
    [Tooltip("Back button")]
    public Button buttonBack;
    
    
    /* Cached references */
    
    /// Array of save slot buttons
    private Button[] saveSlotButtons;
    

    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(saveSlotParameters != null, this, "[SaveSlotMenu] Awake: Save Slot Parameters not set on {0}", this);
        Debug.AssertFormat(levelDataList != null, this, "[SaveSlotMenu] Awake: Level Data List not set on {0}", this);
        #endif
        
        saveSlotButtons = new Button[saveSlotParameters.saveSlotsCount];

        buttonBack.onClick.AddListener(GoBack);
    }

    private void OnDestroy()
    {
        if (buttonBack)
        {
            buttonBack.onClick.RemoveAllListeners();
        }
        
        // don't bother removing listeners from dynamic buttons, as we'd need to check if each of them exists
        // RemoveAllListeners has always been a bonus anyway, since the buttons have always been children of the
        // object with the script adding the listeners, and therefore they'd be destroyed together with the parent object
        // of course, make sure that saveSlotsParent is really on a child
    }
    
    public override void Show()
    {
        gameObject.SetActive(true);
        
        UIPoolHelper.LazyInstantiateWidgets(saveSlotContainerButtonPrefab, saveSlotParameters.saveSlotsCount, saveSlotsParent);

        for (int i = 0; i < saveSlotParameters.saveSlotsCount; i++)
        {
            saveSlotButtons[i] = saveSlotsParent.GetChild(i).GetComponentOrFail<Button>();
            saveSlotButtons[i].onClick.AddListener(StartGame);
        }
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override bool ShouldShowTitle()
    {
        return false;
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
                Debug.LogErrorFormat(levelDataList, "[SaveSlotMenu] StartGame: Level Data List first entry is null");
            }
        }
        else
        {
            Debug.LogErrorFormat(levelDataList, "[SaveSlotMenu] StartGame: Level Data List is empty");
        }
    }

    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
