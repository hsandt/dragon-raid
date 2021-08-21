using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Script to attach to Canvas Pause Menu
/// Tag: CanvasPauseMenu 
public class CanvasPauseMenu : MonoBehaviour
{
    [Header("Scene references")]

    [Tooltip("Resume button")]
    public Button buttonResume;

    [Tooltip("Options button")]
    public Button buttonOptions;

    [Tooltip("Exit button")]
    public Button buttonExit;


    private void Awake()
    {
        buttonResume.onClick.AddListener(ResumeGame);
        buttonOptions.onClick.AddListener(ShowOptions);
        buttonExit.onClick.AddListener(ExitGame);
    }
    
    private void OnDestroy()
    {
        if (buttonResume)
        {
            buttonResume.onClick.RemoveAllListeners();
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
    
    public void Show()
    {
        gameObject.SetActive(true);
        
//        EventSystem.current.SetSelectedGameObject(buttonResume.gameObject);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    // Button callbacks

    private void ResumeGame()
    {
        Hide();
        InGameManager.Instance.ResumeGame();
    }

    private void ShowOptions()
    {
        // TODO: show options sub-menu
    }

    private void ExitGame()
    {
        // TODO: prompt exit confirmation
        InGameManager.ExitToTitleMenu();
    }
}
