using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using CommonsHelper;

public class MainMenu_UITK : MonoBehaviour
{
    [Header("Assets")]
    
    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;
    
    
    /* Components */
    
    /// Root element
    private VisualElement m_RootElement;
    private Button m_ButtonStart;
    private Button m_ButtonOptions;
    private Button m_ButtonExit;
    

    private void OnEnable()
    {
        // Store references to UI Document elements
        m_RootElement = this.GetComponentOrFail<UIDocument>().rootVisualElement;
        m_ButtonStart = m_RootElement.Q<Button>("ButtonStart");
        m_ButtonOptions = m_RootElement.Q<Button>("ButtonOptions");
        m_ButtonExit = m_RootElement.Q<Button>("ButtonExit");
        
        // Bind button actions
        m_ButtonStart.clicked += StartGame;
        m_ButtonOptions.clicked += ShowOptions;
        m_ButtonExit.clicked += ExitGame;
        
        // Currently doesn't work, sent bug report
        m_ButtonStart.Focus();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(levelDataList != null, this, "[MainMenu] Awake: Level Data List not set on {0}", this);
        #endif
    }

    private void OnDisable()
    {
        // Unbind button actions
        m_ButtonStart.clicked -= StartGame;
        m_ButtonOptions.clicked -= ShowOptions;
        m_ButtonExit.clicked -= ExitGame;
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
        Debug.Log("Exit game");

        Application.Quit();
    }
}
