using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using ElRaccoone.Tweens;

/// Script to attach to Canvas Level
/// Tag: CanvasLevel 
public class CanvasLevel : MonoBehaviour
{
    [Header("Child references")]

    [Tooltip("Black overlay")]
    public Image blackOverlay;

    public IEnumerator FadeIn(float duration)
    {
        return blackOverlay.TweenGraphicAlpha(0f, duration).Yield();
    }

    public IEnumerator FadeOut(float duration)
    {
        return blackOverlay.TweenGraphicAlpha(1f, duration).Yield();
    }
}
