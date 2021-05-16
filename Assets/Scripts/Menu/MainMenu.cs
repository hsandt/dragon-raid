using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityConstants;

public class MainMenu : MonoBehaviour
{
    [Header("Assets")]
    
    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;
    
    
    [Header("Scene references")]
    
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
        
        Debug.AssertFormat(levelDataList != null, this, "[MainMenu] Awake: Level Data List not set on {0}", this);
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

    private void ShowOptions()
    {
        Debug.Log("Show options");
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
