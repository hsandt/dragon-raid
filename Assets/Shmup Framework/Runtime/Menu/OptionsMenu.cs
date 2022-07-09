using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        
        buttonBack.Select();
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
        return true;
    }

    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
