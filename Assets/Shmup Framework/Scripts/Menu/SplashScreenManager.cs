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
    [Header("Parameters data")]
    
    [Tooltip("Splash Screen Parameters")]
    public SplashScreenParameters splashScreenParameters;

    
    [Header("Scene references")]
    
    [Tooltip("Team logo")]
    public GameObject teamLogo;


    protected override void Init()
    {
        base.Init();
                
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(splashScreenParameters != null, "No Splash Screen Parameters asset set on Splash Screen Manager", this);
        #endif
    }

    public async Task PlaySplashScreenSequence()
    {
        await teamLogo.TweenGraphicAlpha(1f, splashScreenParameters.logoFadeInDuration).SetFrom(0f).Await();
        await Task.Delay(Mathf.RoundToInt(1000 * splashScreenParameters.logoStayDuration));
        await teamLogo.TweenGraphicAlpha(0f, splashScreenParameters.logoFadeInDuration).Await();
    }
}
