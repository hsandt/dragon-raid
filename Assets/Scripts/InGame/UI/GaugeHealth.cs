using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeHealth : Gauge
{
    [Tooltip("Tracked health system: set it in editor or via script as early as possible (before GaugeHealth.Start)")]
    public HealthSystem trackedHealthSystem;

    private void Start()
    {
        if (trackedHealthSystem != null)
        {
            trackedHealthSystem.RegisterObserver(this);
        }
        RefreshGauge();
    }

    private void OnDestroy()
    {
        if (trackedHealthSystem != null)
        {
            trackedHealthSystem.UnregisterObserver(this);
        }
    }

    protected override float GetRatio()
    {
        return trackedHealthSystem.GetRatio();
    }
    
    protected override string GetValueAsString()
    {
        return trackedHealthSystem.GetValue().ToString("0");
    }
}
