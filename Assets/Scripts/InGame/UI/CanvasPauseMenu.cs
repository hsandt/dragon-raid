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

    [Tooltip("Restart button")]
    public Button buttonRestart;

    [Tooltip("Exit button")]
    public Button buttonExit;


    private void Awake()
    {
        buttonResume.onClick.AddListener(ResumeGame);
        buttonOptions.onClick.AddListener(ShowOptions);
        buttonRestart.onClick.AddListener(RestartLevel);
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
        if (buttonRestart)
        {
            buttonRestart.onClick.RemoveAllListeners();
        }
        if (buttonExit)
        {
            buttonExit.onClick.RemoveAllListeners();
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        
        buttonResume.Select();
    }

    public void Hide()
    {
        EventSystem.current.SetSelectedGameObject(null);

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

    private void RestartLevel()
    {
        Hide();
        // While some games prefer keeping the full pause for menu restart, we allow the game to keep running shortly,
        // but in counterpart we block all game events (esp. damage and finish level) during that transition.
        // Note that this means that CanPlayRestartLevelSequence checking game not being paused is correct.
        InGameManager.Instance.ResumeGame();
        InGameManager.Instance.PlayMenuRestartSequence();
    }

    private void ExitGame()
    {
        // TODO: prompt exit confirmation
        InGameManager.ExitToTitleMenu();
    }
}
