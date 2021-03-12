using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Gauge : MonoBehaviour
{
    [Header("Child references")]
    
    [Tooltip("Gauge Fill Rect Transform")]
    public RectTransform fillRectTransform;

    [Tooltip("Text Widget displaying the value")]
    public TextMeshProUGUI valueTextWidget;

    
    /// Return value ratio to display gauge fill properly
    protected abstract float GetRatio();

    /// Return string to display to represent value
    protected abstract string GetValueAsString();
    
    /// Refresh gauge to reflect value change
    /// Call it on Start after identifying the value to track, and on every value change.
    public void RefreshGauge()
    {
        if (fillRectTransform != null)
        {
            fillRectTransform.anchorMax = new Vector2(GetRatio(), 1f);
        }

        if (valueTextWidget != null)
        {
            valueTextWidget.text = GetValueAsString();
        }
    }
}
