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


    [Header("Parameters")]

    [SerializeField, Tooltip("Gauge fill direction")]
    private GaugeDirection fillDirection;


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
            // We only support horizontal gauges, but in both fill directions
            if (fillDirection == GaugeDirection.Right)
            {
                // Anchor Min must be set to 0 in scene/prefab
                fillRectTransform.anchorMax = new Vector2(GetRatio(), 1f);
            }
            else
            {
                // Anchor Max must be set to 1 in scene/prefab
                fillRectTransform.anchorMin = new Vector2(1f - GetRatio(), 0f);
            }
        }

        if (valueTextWidget != null)
        {
            valueTextWidget.text = GetValueAsString();
        }
    }
}
