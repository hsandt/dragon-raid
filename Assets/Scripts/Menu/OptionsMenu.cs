using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityConstants;

public class OptionsMenu : Menu
{
    [Header("Child references")]
    
    [Tooltip("Back button")]
    public Button buttonBack;


    private void Awake()
    {
        buttonBack.onClick.AddListener(GoBack);
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
        return true;
    }

    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
