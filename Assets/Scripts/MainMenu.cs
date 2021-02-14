using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityConstants;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    
    [Tooltip("Start button")]
    public Button buttonStart;
    
    [Tooltip("Options button")]
    public Button buttonOptions;
    
    [Tooltip("Exit button")]
    public Button buttonExit;


    private void Awake()
    {
        buttonStart.onClick.AddListener(StartGame);
        buttonOptions.onClick.AddListener(ShowOptions);
        buttonExit.onClick.AddListener(ExitGame);
    }

    private void OnDestroy()
    {
        if (buttonStart)
        {
            buttonStart.onClick.RemoveListener(StartGame);
        }
        if (buttonOptions)
        {
            buttonOptions.onClick.RemoveListener(ShowOptions);
        }
        if (buttonExit)
        {
            buttonExit.onClick.RemoveListener(ExitGame);
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene(Scenes.Level);
    }

    private void ShowOptions()
    {
        Debug.Log("Show options");
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
