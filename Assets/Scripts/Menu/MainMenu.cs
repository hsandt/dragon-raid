using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Assets")]
    
    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;
    
    
    [Header("Child references")]
    
    [Tooltip("Play button")]
    public Button buttonPlay;
    
    [Tooltip("Options button")]
    public Button buttonOptions;
    
    [Tooltip("Exit button")]
    public Button buttonExit;
    
    [Tooltip("Play menu")]
    public PlayMenu playMenu;

    [Tooltip("Options menu")]
    public OptionsMenu optionsMenu;


    private void Awake()
    {
        buttonPlay.onClick.AddListener(EnterPlayMenu);
        buttonOptions.onClick.AddListener(EnterOptionsMenu);
        buttonExit.onClick.AddListener(ExitGame);

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(levelDataList != null, this, "[MainMenu] Awake: Level Data List not set on {0}", this);
        #endif
    }

    private void OnDestroy()
    {
        if (buttonPlay)
        {
            buttonPlay.onClick.RemoveAllListeners();
        }
        if (buttonOptions)
        {
            buttonOptions.onClick.RemoveAllListeners();
        }
        if (buttonExit)
        {
            buttonExit.onClick.RemoveAllListeners();
        }
    }
    
    public override void Show()
    {
        gameObject.SetActive(true);
        
        buttonPlay.Select();
    }

    public override void Hide()
    {
        EventSystem.current.SetSelectedGameObject(null);

        gameObject.SetActive(false);
    }

    public override bool ShouldShowTitle()
    {
        return true;
    }

    public override bool CanGoBack()
    {
        return false;
    }

    private void EnterPlayMenu()
    {
        MainMenuManager.Instance.EnterMenu(playMenu);
    }

    private void EnterOptionsMenu()
    {
        MainMenuManager.Instance.EnterMenu(optionsMenu);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
