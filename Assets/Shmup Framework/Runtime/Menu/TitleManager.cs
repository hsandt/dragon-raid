using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityEngine;

using CommonsPattern;

/// Title Manager
public class TitleManager : SingletonManager<TitleManager>
{
    public async void Start()
    {
        await SplashScreenManager.Instance.PlaySplashScreenSequence();
        MainMenuManager.Instance.ShowMainMenu();
    }
}
