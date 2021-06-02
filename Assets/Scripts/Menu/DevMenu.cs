using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevMenu : MonoBehaviour
{
    [Header("Child references")]

    [Tooltip("Level Text")]
    public TextMeshProUGUI m_LevelTMPWidget;
    
    [Tooltip("Exit Button")]
    public Button buttonExit;


    private void Awake()
    {
        buttonExit.onClick.AddListener(ExitGame);
    }

    private void Start()
    {
        m_LevelTMPWidget.text = $"Level {InGameManager.Instance.LevelData.levelIndex:00}";
    }

    private void OnDestroy()
    {
        if (buttonExit)
        {
            buttonExit.onClick.RemoveListener(ExitGame);
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
