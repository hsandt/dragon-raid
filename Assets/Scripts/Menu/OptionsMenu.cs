using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityConstants;

public class OptionsMenu : Menu
{
    [Header("Scene references")]
    
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

    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
