using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevMenu : MonoBehaviour
{
    [Header("Scene references")]
    
    [Tooltip("Exit button")]
    public Button buttonExit;


    private void Awake()
    {
        buttonExit.onClick.AddListener(ExitGame);
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
