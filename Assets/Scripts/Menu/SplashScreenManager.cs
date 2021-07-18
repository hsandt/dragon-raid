using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonsHelper;
using UnityEngine;

using UnityConstants;
using ElRaccoone.Tweens;
using CommonsPattern;

/// Splash Screen Manager
public class SplashScreenManager : SingletonManager<SplashScreenManager>
{
    [Header("Scene references")]
    
    [Tooltip("Team logo")]
    public GameObject teamLogo;

    
    public async Task PlaySplashScreenSequence()
    {
        float duration = 0.1f;
        await teamLogo.TweenGraphicAlpha(1f, duration).SetFrom(0f).SetPingPong().Await();
    }
}
