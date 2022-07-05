using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityConstants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevMenu : MonoBehaviour
{
    [Header("Child references")]

    [Tooltip("Level Text")]
    public TextMeshProUGUI m_LevelTMPWidget;
    
    [Tooltip("Main Menu Button")]
    public Button buttonMainMenu;


    private void Awake()
    {
        buttonMainMenu.onClick.AddListener(GoToMainMenu);
    }

    private void Start()
    {
        m_LevelTMPWidget.text = $"Level {InGameManager.Instance.LevelData.levelIndex:00}";
    }

    private void OnDestroy()
    {
        if (buttonMainMenu)
        {
            buttonMainMenu.onClick.RemoveAllListeners();
        }
    }

    private void GoToMainMenu()
    {
        InGameManager.ExitToTitleMenu();
    }
}
